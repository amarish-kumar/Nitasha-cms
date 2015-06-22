
$(document).ready(function () {

    $('.alert').delay(10000).fadeOut();


    $(".chkSingle").change(function () {
        var checked = 0;
        var total = 0;
        $(".chkSingle").each(function () {
            total = total + 1;
            if ($(this).prop("checked") == true) {
                checked = checked + 1;
            }
        });

        if (total == checked) {
            $('.chkAll').attr('checked', true);
            $('.chkAll').parent().addClass('checked');
        }
        else {
            $('.chkAll').attr('checked', false);
            $('.chkAll').parent().removeClass('checked');
        }
    });


    $("#BtnDeleteMultiple").click(function () {
        var checked = 0;
        $(".chkSingle").each(function () {
            if ($(this).prop("checked") == true) {
                checked++;
            }
        });
        if (checked > 0) {
            $("#modal-confirm").modal('show');
        }
        else {
            alert("Please select at least one record to delete.");
            return false;
        }
    });

    $('.chkAll').change(function () {
        if ($(this).is(":checked")) {
            $('.chkSingle').iCheck('check'); // By Sid- ICheck is Jquery library ..http://fronteed.com/iCheck/   *** this function help to solve check/uncheck issue of checkbox...
            //$('.chkSingle').each(function () {
            //    $(this).prop('checked', true).parent().addClass('checked active');
            //    //$(this).parent().addClass('checked active');
            //});
        }
        else {
            $('input').iCheck('uncheck');
            //$('.chkSingle').each(function () {
            //    $(this).prop('checked', false).parent().removeClass('checked active');
            //    //$(this).parent().removeClass('checked active');
            //});
        }
    });


    //For active tab in leftside menu
    $("#yw0 span").each(function () { $(this).removeClass("high"); });
    $('#yw0').children().each(function () { $(this).removeClass("active"); });
    var currentPage = $("#hdntab").val();

    if (currentPage != "#tab_dashboard" && currentPage != "#tab_payment" && currentPage != "#tab_msfwLabeling" && currentPage != "#tab_reports") {
        $(currentPage).addClass("in");
    }
   
    $(currentPage).parent().addClass("active");
    if (currentPage != "#tab_dashboard" && currentPage != "#tab_payment" && currentPage != "#tab_msfwLabeling" && currentPage != "#tab_reports") {
        $(currentPage).children().addClass("high");
    }
    else {
        $(currentPage).addClass("high");
    }
    // Execute on load
    //checkWidth();

    //Tr mouse enter effect for whole row
    $(".dataTable tbody tr").mouseenter(function () {
        $(this).find("td a:not(.label):not(.btn)").addClass("blue");
       
        
        
    });

    //Tr mouse leave effect for whole row
    $(".dataTable tbody tr").mouseleave(function () {
        $(this).find("td a").removeClass("blue");
        
        //$(this).find("td").removeClass("blue");
    });

});

function showPassword() {
    $("#modal-confirm").modal('show');
}

function showDownloadPassword() {
    $("#modal-DownloadConfirm").modal('show');
}

//mobile redirects .... Added by Sidhdharajsinh Sodha

// Optimalisation: Store the references outside the event handler:
//var $window = $(window);
//var RequiredRedirect = false;
//var redirecttomobilepage = false;

//var $hdnIsmobilePagerendered = $('#hdnIsmobilePagerendered');
//var $hdnIsMobileRenderRequired = $('#hdnIsMobileRenderRequired');
//var pathname = window.location.pathname;

//function checkWidth() {
//    
//    var windowsize = $window.width();
//    if (pathname.toLowerCase().indexOf("list") >= 0) 
//    {
//        if (pathname.toLowerCase().indexOf("list") >= 0 && pathname.toLowerCase().indexOf("mobile") == -1)
//        {
//            redirecttomobilepage = true;
//        }
//        else {
//            redirecttomobilepage = false;
//        }
        //  alert('check mobile in url :' +pathname.toLowerCase().indexOf("mobile") + ' check varu :'+   redirecttomobilepage);
       
//        if (windowsize <= 767) 
//        {
//            var htmlString = $("#yw0").html();
//            //$("#yw0").html(htmlString.replace("-list.aspx", "-list-mobile.aspx"));
//            $("#yw0").html(htmlString.replace(new RegExp("-list.aspx", 'g'),"-list-mobile.aspx"));
//            if (redirecttomobilepage == true) {
//                window.location.href = pathname.replace("list", "list-mobile");
//            }
//            console.log("Desktop to Mobile");
//        }
//        else if (windowsize > 767) {
//            var htmlString = $("#yw0").html();
//            $("#yw0").html(htmlString.replace(new RegExp("-list-mobile.aspx", 'g'), "-list.aspx"));
//            if(redirecttomobilepage == false)
//            {
//                window.location.href = pathname.replace("list-mobile", "list");
//            }
//            console.log("Mobile to Desktop");
//        }
//    }
//}

//// Bind event listener
//$(window).resize(checkWidth);


//****End of mobile redirects...