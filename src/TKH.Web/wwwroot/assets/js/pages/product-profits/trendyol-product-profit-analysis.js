var TKH = TKH || {};

TKH.ProductProfitAnalysis = (() => {
    'use strict';
    let CONFIG = {};

    const Utils = {
        toDecimal: (val) => {
            if (val === null || val === undefined || val === '') return NaN;
            if (typeof val === 'number') return val;
            let cleaned = val.toString().replace(',', '.').trim();
            return parseFloat(cleaned);
        },
        isFilled: (val) => val !== "" && val !== null && val !== undefined && !isNaN(val),
        formatMoney: (val) => val.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' ₺',
        calculateBase: (amount, vatRate) => amount / (1 + vatRate),
        calculateVat: (amount, vatRate) => amount - (amount / (1 + vatRate))
    };

    const Engine = {
        calculate: (inputs, isExport, isSameDay) => {
            const { salesPrice, purchasePrice, commissionRate, productVatRate, shippingCost, serviceFee } = inputs;
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

            const serviceFeeTotal = isSameDay ? CONFIG.fees.SERVICE_SAMEDAY : serviceFee;
            const serviceFeeVat = Utils.calculateVat(serviceFeeTotal, CONFIG.rates.SERVICE_FEE_VAT);
            const shippingVat = Utils.calculateVat(shippingCost, CONFIG.rates.SHIPPING_VAT);
            const commissionTotal = salesPrice * commissionPercent;
            const commissionVat = Utils.calculateVat(commissionTotal, CONFIG.rates.COMMISSION_VAT);

            let exportTotal = 0, exportVat = 0;
            if (isExport) {
                exportTotal = salesPrice * CONFIG.rates.EXPORT;
                exportVat = Utils.calculateVat(exportTotal, CONFIG.rates.EXPORT_VAT);
            }

            const costVat = Utils.calculateVat(purchasePrice, productVatPercent);
            const deductibleVat = costVat + shippingVat + commissionVat + serviceFeeVat + exportVat;
            const netVatBalance = salesVat - deductibleVat;
            const payableVatExpense = Math.max(0, netVatBalance);

            const platformExpenses = commissionTotal + shippingCost + serviceFeeTotal + exportTotal;
            const taxExpenses = withholdingTax + payableVatExpense;
            const totalExpenses = purchasePrice + platformExpenses + taxExpenses;

            const netProfit = salesPrice - totalExpenses;
            const roi = purchasePrice > 0 ? (netProfit / purchasePrice) * 100 : 0;

            return { netProfit, roi };
        }
    };

   const UI = {
        updateCard: (card) => {
            const $card = $(card);
            const inputs = {
                salesPrice: Utils.toDecimal($card.find('[data-field="salesPrice"]').val()),
                purchasePrice: Utils.toDecimal($card.find('[data-field="purchasePrice"]').val()),
                commissionRate: Utils.toDecimal($card.find('[data-field="commissionRate"]').val()),
                shippingCost: Utils.toDecimal($card.find('[data-field="shippingCost"]').val()),
                productVatRate: Utils.toDecimal(card.dataset.vatRate),
                serviceFee: Utils.toDecimal(card.dataset.serviceFee)
            };

            const $results = $card.find('.results-area');
            const $empty = $card.find('.empty-area');
            const $inputRow = $card.find('.calculation-inputs');

            const isValid = Utils.isFilled(inputs.salesPrice) &&
                    Utils.isFilled(inputs.purchasePrice) &&
                    Utils.isFilled(inputs.commissionRate) &&
                    Utils.isFilled(inputs.shippingCost);

            if (!isValid) {
                    $results.addClass('opacity-25');
                    $empty.removeClass('d-none').addClass('d-flex');
                    return;
                }

            $results.removeClass('opacity-25');
            $empty.addClass('d-none').removeClass('d-flex');

            ['NORMAL', 'SAMEDAY', 'EXPORT'].forEach(type => {
                const res = Engine.calculate(inputs, type === 'EXPORT', type === 'SAMEDAY');
                const $profit = $card.find(`[data-calc-result="profit-${type}"]`);
                const $roi = $card.find(`[data-calc-result="roi-${type}"]`);

                $profit.text(Utils.formatMoney(res.netProfit));
                $roi.text('% ' + res.roi.toFixed(2));

                // Renk yönetimi
                if (res.netProfit > 0) {
                    $profit.removeClass('text-danger').addClass('text-gray-800');
                    $roi.removeClass('badge-light-danger').addClass('badge-light-success');
                } else {
                    $profit.removeClass('text-gray-800').addClass('text-danger');
                    $roi.removeClass('badge-light-success').addClass('badge-light-danger');
                }
            });
        }
    };
    return {
        init: () => {
            const $container = $('#profit-analysis-container');
            if (!$container.length) return;
            CONFIG = {
                rates: {
                    WITHHOLDING: Utils.toDecimal($container.data('rate-withholding')) / 100,
                    SHIPPING_VAT: Utils.toDecimal($container.data('rate-shipping-vat')) / 100,
                    COMMISSION_VAT: Utils.toDecimal($container.data('rate-commission-vat')) / 100,
                    SERVICE_FEE_VAT: Utils.toDecimal($container.data('rate-service-vat')) / 100,
                    EXPORT: Utils.toDecimal($container.data('rate-export')) / 100,
                    EXPORT_VAT: Utils.toDecimal($container.data('rate-export-vat')) / 100
                },
                fees: {
                    SERVICE_STANDARD: Utils.toDecimal($container.data('fee-standard')),
                    SERVICE_SAMEDAY: Utils.toDecimal($container.data('fee-sameday'))
                }
            };
            $('.profit-card').each(function() { UI.updateCard(this); });
            $container.on('keyup change', '.calc-input', function() { UI.updateCard($(this).closest('.profit-card')[0]); });
        }
    };
})();
$(document).ready(() => TKH.ProductProfitAnalysis.init());
