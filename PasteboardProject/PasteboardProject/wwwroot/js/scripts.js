function addElement()
{
    let allShowHideElements = document.querySelectorAll('.show-hide-item')
    for (let i = 0; i < allShowHideElements.length; i++)
    {
        if (allShowHideElements[i].style.display === "block") continue;
        if (allShowHideElements[i].style.display === "none")
        {
            allShowHideElements[i].style.display = "block"
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