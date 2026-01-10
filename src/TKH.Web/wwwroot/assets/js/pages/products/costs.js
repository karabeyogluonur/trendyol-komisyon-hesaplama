var TKH = TKH || {};

TKH.ProductCosts = (() => {
    'use strict';

    const state = {
        changedRows: new Set(),
        originals: new Map(),
        saveUrl: ''
    };

    const Utils = {
        getValue: ($row, field) =>{
            const val = $row.find(`[data-field="${field}"]`).val();
            return (val === "" || val === null) ? null : val;
        },
        setValue: ($row, field, val) => $row.find(`[data-field="${field}"]`).val(val),
        toStandardStr: (val) => {
            if (val === "" || val === null || val === undefined) return "";
            let n = parseFloat(val);
            if (isNaN(n)) return "";
            return n.toFixed(2);
        },
        getNullableFloat: ($row, field) => {
            const val = Utils.getValue($row, field);
            return val === null ? null : parseFloat(val);
        }
    };

    const UI = {
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

        checkPriceValidation: ($row) => {
            const $input = $row.find('[data-field="purchasePrice"]');
            const val = $input.val().trim();

            if (val !== "") {
                $input.removeClass('border-danger border-dashed bg-light-danger').addClass('form-control-solid');
            } else {
                $input.addClass('border-danger border-dashed bg-light-danger').removeClass('form-control-solid');
            }
        },

        updateBulkContainer: () => {
            const $container = $('#bulkSaveContainer');
            const $count = $('#changeCount');

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

                            state.originals.set(item.id, {
                                purchasePrice: Utils.toStandardStr(item.purchasePrice),
                                manualCommissionRate: Utils.toStandardStr(item.manualCommissionRate),
                                manualShippingCost: Utils.toStandardStr(item.manualShippingCost)
                            });

                            UI.toggleRowState($row, false);
                            UI.checkPriceValidation($row);
                        });

                        toastr.success('Başarıyla kaydedildi.');
                    } else {
                        toastr.error(res.message || 'Hata oluştu.');
                    }
                },
                complete: () => {
                    $btn.attr('disabled', false);
                    UI.updateBulkContainer();
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

            $('.cost-row').each(function() {
                const $row = $(this);
                state.originals.set($row.data('product-id'), {
                    purchasePrice: Utils.toStandardStr(Utils.getValue($row, 'purchasePrice')),
                    manualCommissionRate: Utils.toStandardStr(Utils.getValue($row, 'manualCommissionRate')),
                    manualShippingCost: Utils.toStandardStr(Utils.getValue($row, 'manualShippingCost'))
                });
            });

            $table.on('input change', '.cost-input', function() {
                const $row = $(this).closest('.cost-row');
                const id = $row.data('product-id');
                const org = state.originals.get(id);

                UI.checkPriceValidation($row);

                const hasChange = Utils.toStandardStr(Utils.getValue($row, 'purchasePrice')) !== org.purchasePrice ||
                                 Utils.toStandardStr(Utils.getValue($row, 'manualCommissionRate')) !== org.manualCommissionRate ||
                                 Utils.toStandardStr(Utils.getValue($row, 'manualShippingCost')) !== org.manualShippingCost;

                UI.toggleRowState($row, hasChange);
            });

            $table.on('click', '.btn-save-row', function() {
                const $row = $(this).closest('.cost-row');
                Actions.save([{
                    id: $row.data('product-id'),
                    purchasePrice: Utils.getNullableFloat($row, 'purchasePrice'),
                    manualCommissionRate: Utils.getNullableFloat($row, 'manualCommissionRate'),
                    manualShippingCost: Utils.getNullableFloat($row, 'manualShippingCost')
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
                        purchasePrice: Utils.getNullableFloat($row, 'purchasePrice'),
                        manualCommissionRate: Utils.getNullableFloat($row, 'manualCommissionRate'),
                        manualShippingCost: Utils.getNullableFloat($row, 'manualShippingCost')
                    });
                });
                Actions.save(payload);
            });
        }
    };
})();

$(document).ready(() => TKH.ProductCosts.init());
