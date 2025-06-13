$(document).ready(function () {
    // Add to Wishlist
    $('.add-to-wishlist').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('product-id');
        var button = $(this);

        $.ajax({
            url: '/Wishlist/AddToWishlist',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    button.addClass('active');
                    toastr.success('Product added to wishlist!');
                } else {
                    toastr.error(response.message || 'Failed to add to wishlist');
                }
            },
            error: function () {
                toastr.error('Error adding to wishlist');
            }
        });
    });

    // Remove from Wishlist
    $('.remove-from-wishlist').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('product-id');
        var row = $(this).closest('tr');

        $.ajax({
            url: '/Wishlist/RemoveFromWishlist',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    row.fadeOut(300, function() { 
                        $(this).remove();
                        if ($('tbody tr').length === 0) {
                            location.reload();
                        }
                    });
                    // Update wishlist button state if it exists on the page
                    $('.add-to-wishlist[data-product-id="' + productId + '"]')
                        .removeClass('active');
                    toastr.success('Product removed from wishlist!');
                } else {
                    toastr.error(response.message || 'Failed to remove from wishlist');
                }
            },
            error: function () {
                toastr.error('Error removing from wishlist');
            }
        });
    });

    // Add to Cart
    $('.product-card__cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).attr('href').split('/').pop();
        var button = $(this);

        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    button.addClass('active bg-success text-white');
                    toastr.success('Product added to cart!');
                    
                    // Update cart total if it exists in the layout
                    if (response.cartTotal) {
                        updateCartTotals(response.cartTotal);
                    }
                } else {
                    toastr.error(response.message || 'Failed to add to cart');
                }
            },
            error: function () {
                toastr.error('Error adding to cart');
            }
        });
    });

    // Remove from Cart
    $('.remove-from-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('product-id');
        var row = $(this).closest('tr');

        $.ajax({
            url: '/Cart/RemoveFromCart',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    row.fadeOut(300, function() { 
                        $(this).remove();
                        if ($('tbody tr').length === 0) {
                            location.reload();
                        }
                    });
                    // Update cart button state if it exists on the page
                    $('.product-card__cart[href="/Cart/AddToCart/' + productId + '"]')
                        .removeClass('active bg-success text-white');
                    if (response.cartTotal) {
                        updateCartTotals(response.cartTotal);
                    }
                    toastr.success('Product removed from cart!');
                } else {
                    toastr.error(response.message || 'Failed to remove from cart');
                }
            },
            error: function () {
                toastr.error('Error removing from cart');
            }
        });
    });

    // Update Cart Quantity
    function updateQuantity(productId, quantity) {
        $.ajax({
            url: '/Cart/UpdateQuantity',
            type: 'POST',
            data: { productId: productId, quantity: quantity },
            success: function (response) {
                if (response.success) {
                    if (response.cartTotal) {
                        updateCartTotals(response.cartTotal);
                        // Update individual product total
                        var row = $('.quantity__input[data-product-id="' + productId + '"]').closest('tr');
                        var price = parseFloat(row.find('.product-price').data('price'));
                        var total = (price * quantity).toFixed(2);
                        row.find('.product-total').text('$' + total);
                    }
                } else {
                    toastr.error(response.message || 'Failed to update quantity');
                }
            },
            error: function () {
                toastr.error('Error updating quantity');
            }
        });
    }

    // Quantity Controls
    $('.quantity__minus').click(function() {
        var input = $(this).siblings('.quantity__input');
        var currentValue = parseInt(input.val());
        if (currentValue > 1) {
            input.val(currentValue - 1);
            updateQuantity($(this).data('product-id'), currentValue - 1);
        }
    });

    $('.quantity__plus').click(function() {
        var input = $(this).siblings('.quantity__input');
        var currentValue = parseInt(input.val());
        input.val(currentValue + 1);
        updateQuantity($(this).data('product-id'), currentValue + 1);
    });

    $('.quantity__input').change(function() {
        var value = parseInt($(this).val());
        if (value < 1) {
            $(this).val(1);
            value = 1;
        }
        updateQuantity($(this).data('product-id'), value);
    });

    // Helper function to update cart totals
    function updateCartTotals(total) {
        $('.cart-total, .text-gray-900.fw-semibold:contains("$")').text(total);
    }
}); 