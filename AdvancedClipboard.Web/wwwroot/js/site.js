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
        filter: "brightness(" + mult + ")"
      });
    }, 30)
  };

  var paste_and_commit = function (text) {
    if (text.length == 0) {
      window.alert("Please copy some text.");
      return;
    }

    var form = $("#paste-form");
    form.find("#paste-field").val(text);
    form.submit();
  };

  var update_preview = async function () {
    var text;
    try {
      text = await navigator.clipboard.readText();
    } catch (error) {
      return;
    }

    var text = $.trim(text);
    if (clipboardpreview.text() == text) {
      return;
    }

    clipboardpreview.text(text);

    var lines = text.split(/\r\n|\r|\n/).length;
    clipboardpreview.attr("rows", Math.min(lines, 5));
  };

  var clipboardpreview = $(".clipboard-preview");

  $("#paste-button").bind("mouseover", function () {
    update_preview();
  })

  $("main").bind("mouseover", function () {
    update_preview();
  })

  $(window).scroll(function () {
    update_preview();
  });

  $("#paste-field").focusout(function () {
    var text = $(this).val();
    text = $.trim(text);

    var lines = text.split(/\r\n|\r|\n/).length;
    $(this).attr("rows", Math.min(lines, 5));

    $(this).val(text);
  })

  $("#paste-field").focusin(function () {
    $(this).attr("rows", 5);
  })

  $(".paste-button").click(async function () {
    var text = await navigator.clipboard.readText();
    paste_and_commit(text);
  })

  $(".add-to-clipboard").click(async function () {
    var text = $(this).parents(".clipboard-card").find(".clipboard-text").text();
    await navigator.clipboard.writeText(text);
    update_preview();
  });

  $(".clipboard-text").click(async function () {
    var text = $(this).text();
    await navigator.clipboard.writeText(text);
    flash($(this).parents(".clipboard-card"));
    update_preview();
  })

  $(".expand-clipboard").click(function () {
    var text = $(this).parents(".clipboard-card").find(".clipboard-text");
    text.css({
      "-webkit-line-clamp": "unset"
    });
  })

  update_preview();
});