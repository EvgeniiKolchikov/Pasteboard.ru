function addElement()
{
    let allShowHideElements = document.getElementsByClassName("show-hide-element");
    for (let i = 0; i < allShowHideElements.length; i++)
    {
        if (allShowHideElements[i].style.display === "block") continue;
        if (allShowHideElements[i].style.display === "none")
        {
            allShowHideElements[i].style.display = "block";
            return;
        }
    }
}

function deleteElement()
{
    let allShowHideElements = document.getElementsByClassName("show-hide-element");
    for (let i = allShowHideElements.length - 1; i > 0; i--)
    {
        if (allShowHideElements[i].style.display === "none") continue;
        if (allShowHideElements[i].style.display === "block")
        {
            allShowHideElements[i].style.display = "none";
            let inputs = allShowHideElements[i].children;
            for (let j = 0;j < inputs.length;j++ )
            {
                inputs[j].value = "";
            }
            return;
        }
    }
}