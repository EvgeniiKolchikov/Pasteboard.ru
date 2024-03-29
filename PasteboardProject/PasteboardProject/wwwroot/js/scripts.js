function addElement()
{
    let showHideElements = document.querySelectorAll('.show-hide-item')
    for (let i = 0; i < showHideElements.length; i++)
    {
        if (showHideElements[i].style.display === "none")
        {
            showHideElements[i].style.display = "block";
            let element = document.getElementById(showHideElements[i].id + "checkbox");
            element.value = "true";
            element.checked = 1;
            return;
        }
    }
}

function deleteElement(id)
{
    let element = document.getElementById(id.toString());
    element.style.display = "none";
    document.getElementById(id + "fieldname").value = "";
    document.getElementById(id + "fieldvalue").value = "";
    document.getElementById(id + "checkbox").value = "false";
    let itemListParent = document.querySelector('.show-hide-elements');
    itemListParent.insertBefore(element,null);
}

function swapLoginLogout()
{
    let cookieExist = document.cookie.match(/^(.*;)?\s*PasteboardCookie\s*=\s*[^;]+(.*)?$/)
    if (cookieExist == null)
    {
        document.getElementById("non-authorized").style.display = "block";
        document.getElementById("authorized").style.display = "none";
    }
    if (cookieExist == null)
    {
        document.getElementById("non-authorized").style.display = "none";
        document.getElementById("authorized").style.display = "block";
    }
}