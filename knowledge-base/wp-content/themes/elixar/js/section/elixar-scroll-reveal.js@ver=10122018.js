/*!
 * elixar-scroll-reveal.js
 *
 * section animation on scroll in custom home page
 *
 * @package Elixar
 */
jQuery(document).ready(function($) {
	/* Scroll Reveal */
	var scroll_reveal = (enable_sreveal_obj.scroll_reveal);	
	if(scroll_reveal==true){
		(function($) {
			'use strict';
			window.sr= new scrollReveal({
			reset: true,
			move: '50px',
			mobile: true
			});
		})();
	}
});