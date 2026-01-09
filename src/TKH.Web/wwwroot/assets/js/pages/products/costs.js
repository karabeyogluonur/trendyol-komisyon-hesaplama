var TKH = TKH || {};

TKH.ProductCosts = (() => {
    'use strict';

    const state = {
        changedRows: new Set(),
        originals: new Map(),
        saveUrl: ''
    };

    const Utils = {
        getValue: ($row, field) => $row.find(`[data-field="${field}"]`).val(),
        setValue: ($row, field, val) => $row.find(`[data-field="${field}"]`).val(val),
        // Sayıları karşılaştırma için standart formata sokar
        toStandardStr: (val) => {
            let n = parseFloat(val);
            return isNaN(n) ? "0.00" : n.toFixed(2);
        }
    };

    const UI = {
        handleFocus: function() {
            if (parseFloat($(this).val()) === 0) $(this).val('');
        },

        handleBlur: function() {
            if ($(this).val() === '' || $(this).val() === null) $(this).val('0.00');
        },

        toggleRowState: ($row, isChanged) => {
            const productId = $row.data('product-id');
            const $saveBtn = $row.find('.btn-save-row');
            const $resetBtn = $row.find('.btn-reset-row');

            if (isChanged) {
                state.changedRows.add(productId);
                $row.addClass('bg-light-primary');
                $saveBtn.removeClass('d-none');
                $resetBtn.removeClass('d-none');
            } else {
                state.changedRows.delete(productId);
                $row.removeClass('bg-light-primary');
                $saveBtn.addClass('d-none');
                $resetBtn.addClass('d-none');
            }
            UI.updateBulkContainer();
        },

        updateBulkContainer: () => {
            const $container = $('#bulkSaveContainer');
            const $count = $('#changeCount');

            // Eğer değişen satır kalmadıysa çubuğu gizle
            if (state.changedRows.size > 0) {
                $container.removeClass('d-none').show();
                $count.text(state.changedRows.size);
            } else {
                $container.hide().addClass('d-none');
            }
        }
    };

    const Actions = {
        save: (dataList) => {
            const $btn = $('#btnBulkSave');
            $btn.attr('disabled', true);

            $.ajax({
                url: state.saveUrl,
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(dataList),
                success: (res) => {
                    if (res.success) {
                        dataList.forEach(item => {
                            const $row = $(`.cost-row[data-product-id="${item.id}"]`);

                            // 1. Yeni değerleri orijinal olarak güncelle (HATA FIX)
                            state.originals.set(item.id, {
                                purchasePrice: Utils.toStandardStr(item.purchasePrice),
                                manualCommissionRate: Utils.toStandardStr(item.manualCommissionRate),
                                manualShippingCost: Utils.toStandardStr(item.manualShippingCost)
                            });

                            // 2. Satır durumunu ve Set listesini temizle
                            UI.toggleRowState($row, false);
                        });

                        toastr.success('Başarıyla kaydedildi.');
                    } else {
                        toastr.error(res.message || 'Hata oluştu.');
                    }
                },
                complete: () => {
                    $btn.attr('disabled', false);
                    UI.updateBulkContainer(); // Çubuğu tekrar kontrol et
                },
                error: () => toastr.error('Bağlantı hatası.')
            });
        }
    };

    return {
        init: () => {
            const $table = $('#costTable');
            if (!$table.length) return;
            state.saveUrl = $table.data('save-url');

            // Başlangıç değerlerini standart formatta sakla
            $('.cost-row').each(function() {
                const $row = $(this);
                state.originals.set($row.data('product-id'), {
                    purchasePrice: Utils.toStandardStr(Utils.getValue($row, 'purchasePrice')),
                    manualCommissionRate: Utils.toStandardStr(Utils.getValue($row, 'manualCommissionRate')),
                    manualShippingCost: Utils.toStandardStr(Utils.getValue($row, 'manualShippingCost'))
                });
            });

            $table.on('focus', '.cost-input', UI.handleFocus);
            $table.on('blur', '.cost-input', UI.handleBlur);

            $table.on('input change', '.cost-input', function() {
                const $row = $(this).closest('.cost-row');
                const id = $row.data('product-id');
                const org = state.originals.get(id);

                // Mevcut değerleri standart formatta karşılaştır
                const hasChange = Utils.toStandardStr(Utils.getValue($row, 'purchasePrice')) !== org.purchasePrice ||
                                 Utils.toStandardStr(Utils.getValue($row, 'manualCommissionRate')) !== org.manualCommissionRate ||
                                 Utils.toStandardStr(Utils.getValue($row, 'manualShippingCost')) !== org.manualShippingCost;

                UI.toggleRowState($row, hasChange);
            });

            $table.on('click', '.btn-save-row', function() {
                const $row = $(this).closest('.cost-row');
                Actions.save([{
                    id: $row.data('product-id'),
                    purchasePrice: parseFloat(Utils.getValue($row, 'purchasePrice')) || 0,
                    manualCommissionRate: parseFloat(Utils.getValue($row, 'manualCommissionRate')) || 0,
                    manualShippingCost: parseFloat(Utils.getValue($row, 'manualShippingCost')) || 0
                }]);
            });

            $table.on('click', '.btn-reset-row', function() {
                const $row = $(this).closest('.cost-row');
                const org = state.originals.get($row.data('product-id'));
                Utils.setValue($row, 'purchasePrice', org.purchasePrice);
                Utils.setValue($row, 'manualCommissionRate', org.manualCommissionRate);
                Utils.setValue($row, 'manualShippingCost', org.manualShippingCost);
                UI.toggleRowState($row, false);
            });

            $('#btnBulkSave').on('click', () => {
                const payload = [];
                state.changedRows.forEach(id => {
                    const $row = $(`.cost-row[data-product-id="${id}"]`);
                    payload.push({
                        id: id,
                        purchasePrice: parseFloat(Utils.getValue($row, 'purchasePrice')) || 0,
                        manualCommissionRate: parseFloat(Utils.getValue($row, 'manualCommissionRate')) || 0,
                        manualShippingCost: parseFloat(Utils.getValue($row, 'manualShippingCost')) || 0
                    });
                });
                Actions.save(payload);
            });
        }
    };
})();

$(document).ready(() => TKH.ProductCosts.init());
