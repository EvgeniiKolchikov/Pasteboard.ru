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
    let inputs = element.children;
    for (let i = 0; i < inputs.length; i++)
    {
        inputs[i].value = "";
    }
    let itemListParent = document.querySelector('.show-hide-elements');
    let itemList = document.querySelectorAll('.show-hide-item');
    itemListParent.insertBefore(element,null);
}