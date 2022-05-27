﻿$(document).ready(function () {

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
        filter: "brightness(" + mult + ")"
      });
    }, 30)
  };

  $(".paste-button").click(async function () {
    var text = await navigator.clipboard.readText();
    if (text.length == 0) {
      window.alert("Please copy some text.");
      return;
    }
    var form = $(this).parents("#PasteForm");
    form.find("#ContentToAdd").val(text);
    form.submit();
  });

  $(".add-to-clipboard").click(async function () {
    var text = $(this).parents(".clipboard-card").find(".clipboard-text").text();
    await navigator.clipboard.writeText(text);
  });

  $(".clipboard-text").click(async function () {
    var text = $(this).text();
    await navigator.clipboard.writeText(text);
    flash($(this).parents(".clipboard-card"));
  });

  $(".expand-clipboard").click(function () {
    var text = $(this).parents(".clipboard-card").find(".clipboard-text");
    text.css({
      "-webkit-line-clamp": "unset"
    });
  });

});