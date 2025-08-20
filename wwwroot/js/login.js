$(document).ready(function() {
    // Form validation
    $('#loginForm').on('submit', function(e) {
        var isValid = true;
        var username = $('#Username').val().trim();
        var password = $('#Password').val().trim();
        
        // Clear previous errors
        $('.field-validation-error').remove();
        $('.is-invalid').removeClass('is-invalid');
        
        // Validate username
        if (!username) {
            $('#Username').addClass('is-invalid');
            $('#Username').after('<span class="field-validation-error">Tên đăng nhập là bắt buộc</span>');
            isValid = false;
        }
        
        // Validate password
        if (!password) {
            $('#Password').addClass('is-invalid');
            $('#Password').after('<span class="field-validation-error">Mật khẩu là bắt buộc</span>');
            isValid = false;
        }
        
        if (isValid) {
            // Show loading state
            var $btn = $('.btn-login');
            var originalText = $btn.html();
            
            $btn.addClass('loading').html('<i class="fas fa-spinner fa-spin"></i> Đang đăng nhập...');
            $btn.prop('disabled', true);
            
            // Re-enable button after 3 seconds if no response
            setTimeout(function() {
                if ($btn.hasClass('loading')) {
                    $btn.removeClass('loading').html(originalText);
                    $btn.prop('disabled', false);
                }
            }, 3000);
        } else {
            e.preventDefault();
        }
    });
    
    // Real-time validation
    $('#Username').on('input', function() {
        var value = $(this).val().trim();
        if (value) {
            $(this).removeClass('is-invalid').addClass('is-valid');
            $(this).next('.field-validation-error').remove();
        } else {
            $(this).removeClass('is-valid');
        }
    });
    
    $('#Password').on('input', function() {
        var value = $(this).val().trim();
        if (value) {
            $(this).removeClass('is-invalid').addClass('is-valid');
            $(this).next('.field-validation-error').remove();
        } else {
            $(this).removeClass('is-valid');
        }
    });
    
    // Enter key to submit
    $('input').on('keypress', function(e) {
        if (e.which === 13) {
            $('#loginForm').submit();
        }
    });
    
    // Focus on username field on page load
    $('#Username').focus();
    
    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
    
    // Password visibility toggle
    $('#togglePassword').on('click', function() {
        var passwordField = $('#Password');
        var icon = $(this).find('i');
        
        if (passwordField.attr('type') === 'password') {
            passwordField.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passwordField.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });
});
