(function () {
    const data = window.dashboardData || { categories: [], statuses: [] };

    const doughnutOptions = {
        responsive: true,
        maintainAspectRatio: false,
        cutout: "72%",
        plugins: {
            legend: { display: false },
            tooltip: {
                callbacks: {
                    label: function (context) {
                        return " " + context.label + ": " + context.parsed;
                    }
                }
            }
        }
    };

    function createChart(canvasId, items, labelKey, valueKey) {
        const canvas = document.getElementById(canvasId);
        if (!canvas || typeof Chart === "undefined") {
            return;
        }

        const labels = items.map(function (item) { return item[labelKey]; });
        const values = items.map(function (item) { return item[valueKey]; });
        const colors = items.map(function (item) { return item.color; });

        new Chart(canvas, {
            type: "doughnut",
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: colors,
                    borderWidth: 0,
                    hoverOffset: 4
                }]
            },
            options: doughnutOptions
        });
    }

    createChart("categoryChart", data.categories || [], "categoryName", "count");
    createChart("statusChart", data.statuses || [], "status", "count");
})();
