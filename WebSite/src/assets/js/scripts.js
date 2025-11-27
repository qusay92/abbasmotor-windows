!(function (s) {
    "use strict";
    document.addEventListener("DOMContentLoaded", function () {
      window.addEventListener("scroll", function () {
        window.scrollY > 50
          ? document.getElementById("navbar_top").classList.add("fixed-top")
          : (document.getElementById("navbar_top").classList.remove("fixed-top"),
            (document.body.style.paddingTop = "0"));
      });
    });
})(jQuery);

$('.navbar-nav li').click(function(){
  $('.navbar-nav li').removeClass('active');
  $(this).addClass('active');
})
$('.nav-link').click(function(){
  $('.navbar-collapse').removeClass('show');
})
$('#topbutton').click(function(){
  $('.navbar-nav li').removeClass('active');
  $('.navbar-nav li:first-child').addClass('active');
})

$('.bottom-navbar li').click(function(){
  $('.bottom-navbar li').removeClass('active');
  $(this).addClass('active');
})

var svg = $("#topbutton");
$(window).scroll(function () {
  $(window).scrollTop() > 300 ? svg.addClass("show") : svg.removeClass("show");
}),
svg.on("click", function (svg) {
    //svg.preventDefault(),
    $("html, body").animate({ scrollTop: 0, behavior: "smooth" }, "1000");
}),
$(window).load(function () {
  $("body").addClass("page-loaded");
});

var topMenu = $("#navbar_top"),
    topMenuHeight = topMenu.outerHeight()+15,
    // All list items
    menuItems = topMenu.find("a"),
    // Anchors corresponding to menu items
    scrollItems = menuItems.map(function(){
      var item = $($(this).attr("href"));
      if (item.length) { return item; }
    });

// Bind to scroll
$(window).scroll(function(){
   var fromTop = $(this).scrollTop()+topMenuHeight;
   var cur = scrollItems.map(function(){
     if ($(this).offset().top < fromTop)
       return this;
   });
   cur = cur[cur.length-1];
   var id = cur && cur.length ? cur[0].id : "";
   menuItems
     .parent().removeClass("active")
     .end().filter("[href='#"+id+"']").parent().addClass("active");
});



$("a.nav-link").click(function() {
  //event.preventDefault();
  $("html, body").animate({
      scrollTop: $($(this).attr("href")).offset().top - 95
    }, 100);
});
$("a.nav-link.about-item").click(function(event) {
  event.preventDefault();
  $("html, body").animate({
      scrollTop: $($(this).attr("href")).offset().top - 220
    }, 100);
});

wow = new WOW({
    boxClass: 'wow', // default
    animateClass: 'animated', // default
    offset: 0, // default
    mobile: false, // default
    live: true // default
})
wow.init();

$('.modal').on('shown.bs.modal', function (e) {
  $('.slider-info').resize();
  $('.slider-info').slick('refresh');
})
$('.modal').on("hidden.bs.modal", function (e) { 
  if ($('.modal:visible').length) { 
      $('body').addClass('modal-open');
  }
});

var collapseElementList = [].slice.call(document.querySelectorAll('.collapse'))
var collapseList = collapseElementList.map(function (collapseEl) {
  return new bootstrap.Collapse(collapseEl)
})

$("#togglePassword").click(function() {

  var input = $($(this).attr("toggle"));
  $(this).toggleClass("mdi-eye-off");
  const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
  password.setAttribute('type', type);
});

$("#toggleclientPassword").click(function() {
 
  var input = $($(this).attr("toggle"));
  $(this).toggleClass("mdi-eye-off");
  const type = clientpassword.getAttribute('type') === 'password' ? 'text' : 'password';
  clientpassword.setAttribute('type', type);
});

$("#toggleconfirmPassword").click(function() {
  var input = $($(this).attr("toggle"));
  $(this).toggleClass("mdi-eye-off");
  const type = confirmpassword.getAttribute('type') === 'password' ? 'text' : 'password';
  confirmpassword.setAttribute('type', type);
});

