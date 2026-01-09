var TKH = TKH || {};

TKH.CommissionCalculator = (() => {
    'use strict';

    let CONFIG = {};
    let CHARTS = { revenue: null, expense: null };

    const Utils = {
        parseMoney: (val) => {
            if (!val) return 0;
            if (typeof val === 'number') return val;
            const cleaned = val.toString().replace(/\./g, '').replace(',', '.');
            return parseFloat(cleaned) || 0;
        },
        formatMoney: (amount) => {
            return new Intl.NumberFormat('tr-TR', {
                style: 'currency',
                currency: 'TRY',
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }).format(amount);
        },
        calculateBase: (amount, vatRate) => amount / (1 + vatRate),
        calculateVat: (amount, vatRate) => amount - (amount / (1 + vatRate))
    };

    const Logic = {
        calculate: (inputs) => {
            const { salesPrice, purchasePrice, commissionRate, productVatRate, shippingCost, isExport, isSameDay } = inputs;
            const productVatPercent = productVatRate / 100;
            const commissionPercent = commissionRate / 100;

            let salesExclVat, salesVat;
            if (isExport) {
                salesExclVat = salesPrice;
                salesVat = 0;
            } else {
                salesExclVat = Utils.calculateBase(salesPrice, productVatPercent);
                salesVat = Utils.calculateVat(salesPrice, productVatPercent);
            }

            const withholdingTax = Utils.calculateBase(salesPrice, productVatPercent) * CONFIG.rates.WITHHOLDING;
            const serviceFeeTotal = isSameDay ? CONFIG.fees.SERVICE_SAMEDAY : CONFIG.fees.SERVICE_STANDARD;
            const serviceFeeBase = Utils.calculateBase(serviceFeeTotal, CONFIG.rates.SERVICE_FEE_VAT);
            const serviceFeeVat = Utils.calculateVat(serviceFeeTotal, CONFIG.rates.SERVICE_FEE_VAT);
            const shippingBase = Utils.calculateBase(shippingCost, CONFIG.rates.SHIPPING_VAT);
            const shippingVat = Utils.calculateVat(shippingCost, CONFIG.rates.SHIPPING_VAT);
            const commissionTotal = salesPrice * commissionPercent;
            const commissionBase = Utils.calculateBase(commissionTotal, CONFIG.rates.COMMISSION_VAT);
            const commissionVat = Utils.calculateVat(commissionTotal, CONFIG.rates.COMMISSION_VAT);

            let exportTotal = 0, exportBase = 0, exportVat = 0;
            if (isExport) {
                exportTotal = salesPrice * CONFIG.rates.EXPORT;
                exportBase = Utils.calculateBase(exportTotal, CONFIG.rates.EXPORT_VAT);
                exportVat = Utils.calculateVat(exportTotal, CONFIG.rates.EXPORT_VAT);
            }

            const costBase = Utils.calculateBase(purchasePrice, productVatPercent);
            const costVat = Utils.calculateVat(purchasePrice, productVatPercent);
            const deductibleVat = costVat + shippingVat + commissionVat + serviceFeeVat + exportVat;
            const netVatBalance = salesVat - deductibleVat;
            const payableVatExpense = Math.max(0, netVatBalance);

            const platformExpenses = commissionTotal + shippingCost + serviceFeeTotal + exportTotal;
            const taxExpenses = withholdingTax + payableVatExpense;
            const totalExpenses = purchasePrice + platformExpenses + taxExpenses;

            const netProfit = salesPrice - totalExpenses;
            const roi = purchasePrice > 0 ? (netProfit / purchasePrice) * 100 : 0;

            return {
                financials: {
                    salesPrice, salesVat, salesExclVat, purchasePrice, costVat, costBase,
                    commissionTotal, commissionVat, commissionBase, shippingCost, shippingVat, shippingBase,
                    serviceFeeTotal, serviceFeeVat, serviceFeeBase, exportTotal, exportVat, exportBase,
                    withholdingTax, netVatBalance, netProfit, roi
                },
                charts: {
                    netProfit: Math.max(0, netProfit), purchasePrice, platformExpenses, taxExpenses
                }
            };
        }
    };

    const UI = {
        init: () => {
            const form = $('#kt_calculator_form');
            CONFIG = {
                rates: {
                    WITHHOLDING: parseFloat(form.data('rate-withholding')),
                    SHIPPING_VAT: parseFloat(form.data('rate-shipping-vat')),
                    COMMISSION_VAT: parseFloat(form.data('rate-commission-vat')),
                    EXPORT: parseFloat(form.data('rate-export')),
                    EXPORT_VAT: parseFloat(form.data('rate-export-vat')),
                    SERVICE_FEE_VAT: parseFloat(form.data('rate-service-fee-vat'))
                },
                fees: {
                    SERVICE_STANDARD: parseFloat(form.data('fee-standard')),
                    SERVICE_SAMEDAY: parseFloat(form.data('fee-sameday'))
                }
            };

            $('.money-input').on('blur', function () {
                const val = Utils.parseMoney($(this).val());
                $(this).val(val.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
            });

            $('#chkExport').on('change', function () {
                if ($(this).is(':checked')) $('#chkSameDayShipping').prop('checked', false).prop('disabled', true);
                else $('#chkSameDayShipping').prop('disabled', false);
            });

            $('#btnCalculate').on('click', TKH.CommissionCalculator.process);
        },

        updateResults: (result, isSameDay, isExport) => {
            const f = result.financials;

            $('[data-calc-result]').each(function() {
                const key = $(this).data('calc-result');
                if (f[key] !== undefined) {
                    const formatted = (key === 'roi') ? `% ${f[key].toFixed(2)}` : Utils.formatMoney(f[key]);
                    $(this).text(formatted);
                }
            });

            const vatCard = $('#lblPayableVat').closest('.card');
            if (f.netVatBalance < 0) {
                $('#lblVatTitle').text('Alınacak (Devreden) KDV');
                vatCard.removeClass('bg-warning').addClass('bg-info');
            } else {
                $('#lblVatTitle').text('Devlete Ödenecek KDV');
                vatCard.removeClass('bg-info').addClass('bg-warning');
            }

            $('#lblServiceFeeDesc').text(isSameDay ? 'Aynı Gün Teslimat' : 'Standart Kesinti');
            if (isExport) $('#rowExport').removeClass('d-none'); else $('#rowExport').addClass('d-none');

            $('#resultSection, #chartsSection').removeClass('d-none');
            $('html, body').animate({ scrollTop: $('#resultSection').offset().top - 100 }, 500);
        }
    };

    const Charts = {
        init: (data, totalSales, financials) => {
            const common = { fontFamily: 'Inter', toolbar: { show: false } };
            const colors = ['#50cd89', '#f1416c', '#009ef7', '#ffc700', '#7239ea', '#181C32'];

            const revOptions = {
                series: [data.netProfit, data.purchasePrice, data.platformExpenses, data.taxExpenses].map(v => parseFloat(v.toFixed(2))),
                labels: ['Net Kâr', 'Ürün Maliyeti', 'Pazaryeri Giderleri', 'Vergiler'],
                chart: { type: 'donut', height: 350, ...common },
                colors: colors.slice(0, 4),
                plotOptions: { pie: { donut: { labels: { show: true, total: { show: true, label: 'Ciro', formatter: () => Utils.formatMoney(totalSales) } } } } },
                dataLabels: { enabled: false }
            };

            if (CHARTS.revenue) CHARTS.revenue.destroy();
            CHARTS.revenue = new ApexCharts(document.querySelector('#kt_charts_revenue_distribution'), revOptions);
            CHARTS.revenue.render();

            const expOptions = {
                series: [{ name: 'Tutar', data: [data.purchasePrice, financials.commissionTotal, financials.shippingCost, financials.serviceFeeTotal, data.taxExpenses].map(v => parseFloat(v.toFixed(2))) }],
                chart: { type: 'bar', height: 350, ...common },
                xaxis: { categories: ['Ürün', 'Komisyon', 'Kargo', 'Hizmet', 'Vergi'] },
                colors: colors.slice(1, 6),
                plotOptions: { bar: { borderRadius: 4, distributed: true, columnWidth: '40%' } },
                legend: { show: false }
            };

            if (CHARTS.expense) CHARTS.expense.destroy();
            CHARTS.expense = new ApexCharts(document.querySelector('#kt_charts_expense_breakdown'), expOptions);
            CHARTS.expense.render();
        }
    };

    return {
        init: UI.init,
        process: () => {
            const inputs = {
                salesPrice: Utils.parseMoney($('#salesPrice').val()),
                purchasePrice: Utils.parseMoney($('#purchasePrice').val()),
                commissionRate: parseFloat($('#commissionRate').val()) || 0,
                productVatRate: parseFloat($('#vatRate').val()) || 0,
                shippingCost: Utils.parseMoney($('#shippingCost').val()),
                isExport: $('#chkExport').is(':checked'),
                isSameDay: $('#chkSameDayShipping').is(':checked')
            };

            const results = Logic.calculate(inputs);
            UI.updateResults(results, inputs.isSameDay, inputs.isExport);
            Charts.init(results.charts, inputs.salesPrice, results.financials);
        }
    };
})();

$(document).ready(() => TKH.CommissionCalculator.init());
