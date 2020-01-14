/*!
 * elixar-theme.js
 *
 * custom js
 *
 * @package Elixar
 */
jQuery(document).ready(function($) {
	/* Woocommerce sidebar category collapse */
	if (typeof dcAccordion != 'undefined') {
	$('.woocommerce .product-categories').dcAccordion({
        saveState: false,
        autoExpand: true,
        showCount: true,
    });
	$('.dcjq-icon').click(function(){
		$(this).toggleClass('less');
	});
	}

	/*----  Top Contact Bar Expand ----*/
	jQuery(".e_top_expande").on("click", function () {
		var $thiss = jQuery(this);
		var $conta = $thiss.siblings(".e-contact-right-head");
		if ($thiss.hasClass("not_expanded")) {
			jQuery($conta).stop().slideDown(300, function () {
				$thiss.removeClass("not_expanded");
			});
		} else {
			jQuery($conta).stop().slideUp(300, function () {
				$thiss.addClass("not_expanded");
			});
		}
	});
	/*---- Header Search Icon ----*/
	if( jQuery( '.search-icon' ).length > 0 ) {
		jQuery( '.search-icon' ).on('click', function( e ) {
			e.preventDefault();
			jQuery( '.search-box-wrap' ).slideToggle();
		});
	}
	/*---- Mobile Menu. ----*/
	jQuery( '#mobile-trigger' ).sidr({
		timing: 'ease-in-out',
		speed: 500,
		source: '#mob-menu',
		name: 'sidr-main'
	});
	jQuery( '#mobile-trigger-quick' ).sidr({
		timing: 'ease-in-out',
		side: 'right',
		speed: 500,
		source: '#mob-menu-quick',
		name: 'sidr-quick'
	});
	!0 === jQuery("body").hasClass("enabled-sticky-primary-menu") && jQuery(window).scroll(function() {
		jQuery(window).scrollTop() > jQuery("#e_main_nav").offset().top && !jQuery("#e_main_nav").hasClass("fixed") ? jQuery("#e_main_nav").addClass("fixed") : 0 === jQuery(window).scrollTop() && jQuery("#e_main_nav").removeClass("fixed")
	});
	jQuery('li.sidr-class-dropdown-submenu a, li.sidr-class-dropdown a').addClass('disabled'); 
	jQuery('#carousel-30').find('.item').first().addClass('active');
	/*---- Back To Top ----*/
	var to_top_offset = 300,
	to_top_offset_opacity = 1200,
	scroll_top_duration = 900,
	$back_to_top = jQuery('#scroll-top, #scroll-top-fixed, #scroll-top-fixed-left, #scroll-top-left, #scroll-top-right, #scroll-top-center');
	jQuery(window).scroll(function () {
		if (jQuery(this).scrollTop() > to_top_offset) {
			$back_to_top.addClass('scroll-top_visible');
		} else {
			$back_to_top.removeClass('scroll-top_visible elixar_go_fade-out');
		}
		if (jQuery(this).scrollTop() > to_top_offset_opacity) {
			$back_to_top.addClass('elixar_go_fade-out');
		}
		return false;
	});
	$back_to_top.on('click', function (event) {
		event.preventDefault();
		jQuery('body,html').animate({
		scrollTop: 0,
		//easing : "easeOutElastic"
		}, {queue: false, duration: scroll_top_duration}
		);
	});
});