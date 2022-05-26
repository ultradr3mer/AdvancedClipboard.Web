$(document).ready(function () {

  var flash = function (elements) {
    var mult = 2.0;
    var color = "255, 255, 20" // has to be in this format since we use rgba
    var interval = setInterval(function () {
      mult *= 0.97;
      mult -= 0.02;
      if (mult <= 1) {
        $(elements).css({
          filter: "none"
        });
        clearInterval(interval);
      }
      $(elements).css({
        filter: "brightness(" + mult + ")" });
    }, 30)
  };

  var form = $("#PasteForm");
  form.submit(function () {
    event.preventDefault();
    form.find("#ContentToAdd").val(navigator.clipboard.readText());
    $(this).unbind('submit').submit();
  });

  $(".add-to-clipboard").click(function () {
    var text = $(this).parents(".clipboard-card").find(".clipboard-text").text();
    navigator.clipboard.writeText(text);
  });

  //$(".clipboard-card").click(function () {
  //  var text = $(this).find(".clipboard-text").text();
  //  navigator.clipboard.writeText(text);
  //});

  $(".clipboard-text").click(function () {
    var text = $(this).text();
    navigator.clipboard.writeText(text);
    flash($(this).parents(".clipboard-card"));
  });

});