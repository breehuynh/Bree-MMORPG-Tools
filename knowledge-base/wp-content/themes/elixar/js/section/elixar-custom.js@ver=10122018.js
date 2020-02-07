/*!
 * elixar-custom.js
 *
 * Add class to widgets
 *
 * @package Elixar
 */
jQuery(document).ready(function(){
	jQuery(".elixar-product-sidebar.widget_recent_entries ul, .elixar-product-sidebar.widget_recent_comments ul, .elixar-product-sidebar.widget_pages ul, .elixar-product-sidebar.widget_nav_menu ul, .elixar-product-sidebar.widget_categories ul, .elixar-product-sidebar.widget_archive ul, .elixar-product-sidebar.widget_meta ul").addClass("list-unstructured");
	jQuery("#section_footer .widget_recent_entries ul, #section_footer .widget_recent_comments ul, #section_footer .widget_pages ul, #section_footer .widget_nav_menu ul, #section_footer .widget_categories ul, #section_footer .widget_archive ul, #section_footer .widget_meta ul").addClass("list-unstructured");
});