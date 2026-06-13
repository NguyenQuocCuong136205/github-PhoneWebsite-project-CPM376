$(document).ready(function () {
    $('.update-qty-btn').on('click', function (e) {
        e.preventDefault();
        var urlStr = $(this).attr('href');
        var qtyInput = $(this).closest('td').find('.xc-cart-input');
        var qty = qtyInput.val();
        
        console.log("Update Clicked. Original URL:", urlStr);
        console.log("Found Input:", qtyInput, "New Quantity Value:", qty);
        
        if (qty !== undefined) {
            var url = new URL(urlStr, window.location.origin);
            url.searchParams.set('quantity', qty);
            var finalUrl = url.pathname + url.search;
            console.log("Redirecting to:", finalUrl);
            window.location.href = finalUrl;
        }
    });
});
