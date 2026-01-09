const CargoApp = {
    data: null,
    config: {
        jsonPath: '/data/cargo-prices.json',
        colors: ['primary', 'warning', 'success', 'danger', 'info', 'dark']
    },

    async init() {
        try {
            const response = await fetch(this.config.jsonPath);
            if (!response.ok) throw new Error('Veri dosyası yüklenemedi!');
            this.data = await response.json();

            const btn = document.getElementById('btnCalculateDesi');
            if (btn) {
                btn.addEventListener('click', () => this.processCalculation());
            }
        } catch (error) {
            throw new Error('Veri dosyası yüklenemedi!');
        }
    },

    formatCurrency(amount) {
        return new Intl.NumberFormat('tr-TR', {
            style: 'currency', currency: 'TRY', minimumFractionDigits: 2
        }).format(amount);
    },

    calculateDesi(w, l, h) {
        const divisor = this.data.metadata.settings.desiDivisor || 3000;
        return (w * l * h) / divisor;
    },

    getPrice(carrierName, desi, salesPrice) {
        const roundedDesi = Math.ceil(desi);
        const settings = this.data.metadata.settings;

        if (roundedDesi <= settings.baremDesiLimit && salesPrice > 0 && salesPrice < settings.baremPriceLimit) {
            const barem = this.data.baremPricing.find(b => salesPrice >= b.salesMin && salesPrice <= b.salesMax);
            if (barem && barem.prices[carrierName]) return barem.prices[carrierName];
        }

        const lookupDesi = roundedDesi > settings.maxDesiLimit ? settings.maxDesiLimit : roundedDesi;
        const rateEntry = this.data.desiRates[lookupDesi.toString()];
        return rateEntry ? rateEntry[carrierName] : null;
    },

    processCalculation() {
        const w = parseFloat(document.getElementById('width').value) || 0;
        const l = parseFloat(document.getElementById('length').value) || 0;
        const h = parseFloat(document.getElementById('height').value) || 0;
        const salesPrice = parseFloat(document.getElementById('salesPrice').value) || 0;

        const calculatedDesi = this.calculateDesi(w, l, h);
        const finalDesi = Math.max(1, calculatedDesi);

        document.getElementById('lblCalculatedDesi').innerText = `${finalDesi.toFixed(2)} Desi`;
        const isBarem = finalDesi <= 10 && salesPrice > 0 && salesPrice < 300;

        const cardPriceType = document.getElementById('cardPriceType');
        const lblBaremType = document.getElementById('lblBaremType');

        if (isBarem) {
            lblBaremType.innerText = "Barem Fiyatı Uygulandı";
            cardPriceType.classList.replace('bg-success', 'bg-warning');
        } else {
            lblBaremType.innerText = "Standart Desi Tarifesi";
            cardPriceType.classList.replace('bg-warning', 'bg-success');
        }

        this.renderTable(finalDesi, salesPrice);
        document.getElementById('resultSection').classList.remove('d-none');
    },

    renderTable(desi, salesPrice) {
        const tableBody = document.getElementById('resultTableBody');
        const template = document.getElementById('carrier-row-template').innerHTML;
        tableBody.innerHTML = '';

        const results = Object.keys(this.data.carrierDetails).map(carrier => {
            return {
                name: carrier,
                price: this.getPrice(carrier, desi, salesPrice),
                details: this.data.carrierDetails[carrier]
            };
        }).sort((a, b) => (a.price || 9999) - (b.price || 9999));

        results.forEach((item, index) => {
            const color = this.config.colors[index % this.config.colors.length];
            let statusBadge = item.price
                ? '<span class="badge badge-light-success">Hizmet Veriyor</span>'
                : '<span class="badge badge-light-danger">Limit Dışı</span>';

            let extrasHtml = item.details.extras.map(ex =>
                `<div class="d-flex align-items-center mb-1"><span class="bullet bullet-dot bg-danger me-2"></span><span class="fs-8 text-gray-600">${ex}</span></div>`
            ).join('') || "Ek maliyet bulunamadı.";

            let rowHtml = template
                .replace(/{carrierName}/g, item.name)
                .replace(/{firstLetter}/g, item.name.charAt(0))
                .replace(/{price}/g, item.price ? this.formatCurrency(item.price) : '-')
                .replace(/{color}/g, color)
                .replace(/{status}/g, statusBadge)
                .replace(/{collapseId}/g, `collapse_row_${index}`)
                .replace(/{extras}/g, extrasHtml);

            tableBody.insertAdjacentHTML('beforeend', rowHtml);
        });
    }
};

document.addEventListener('DOMContentLoaded', () => CargoApp.init());
