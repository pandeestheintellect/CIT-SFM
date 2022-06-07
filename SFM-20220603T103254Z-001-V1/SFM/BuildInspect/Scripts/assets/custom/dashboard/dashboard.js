$(document).ready(function(){
    $('#dashboard-display').DataTable({
        responsive: true
    });

    $(".slick-slider").slick({
        dots: true,
        slidesToShow: 1,
        slidesToScroll: 1
    });
});