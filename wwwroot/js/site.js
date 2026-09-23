document.addEventListener("DOMContentLoaded", function () {

    console.log("Inventory Management System loaded.");

    // Automatically hide Bootstrap alerts after 5 seconds
    const alerts = document.querySelectorAll(".alert");

    alerts.forEach(function (alert) {
        setTimeout(function () {

            if (typeof bootstrap !== "undefined") {
                const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
                bsAlert.close();
            } else {
                alert.style.display = "none";
            }

        }, 5000);
    });

});
