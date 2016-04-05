    function ShowFilePopup(id, filename)
    {
        newwindow=window.open('ShowFile.aspx?id=' + id, filename);
        if (window.focus) {newwindow.focus()}
    }
    
    function InsertFile(id, filename)
    {
        var textbox, sText, sExt;
        
        //check if image attachment or regular file
        sExt = filename.substring(filename.length - 3).toLowerCase();
        if (sExt == "jpg" || sExt == "gif" || sExt == "png")
            sText = "^" + id + "^";
        else
            sText = "^^" + filename + "|" + id + "^^";            
        
        //get the main textbox         
        textbox = document.getElementById("ctl00_ContentPlaceHolder1_DataList1_ctl00_txtPageText");
        textbox.focus();
        if (textbox.document) //IE - can insert into range
        {
            textbox.document.selection.createRange().text = sText;
            textbox.document.selection.createRange().select;
        }
        else if (textbox.selectionStart) //Firefox
        {
            var iStart, iEnd, iScrollTop;
            iScrollTop = textbox.scrollTop;             
            iStart = textbox.selectionStart;
            iEnd = textbox.selectionEnd;                   
            textbox.value = textbox.value.substring(0,iStart) + 
                sText + textbox.value.substring(iEnd,textbox.value.length);   
            textbox.scrollTop = iScrollTop;
        }
        else
        {        
            textbox.value += sText; 
            textbox.scrollTop = textbox.scrollHeight; 
        }
        textbox.focus();
        return false;
    }
    
    function ApplyFormatting(formatString, bNewPage)
    {
        var textbox, myRange, oldText, newText, bIE;
        if (bNewPage)
            textbox = document.getElementById("ctl00_ContentPlaceHolder1_txtNewPage");
        else            
            textbox = document.getElementById("ctl00_ContentPlaceHolder1_DataList1_ctl00_txtPageText"); 
        
        textbox.document?(bIE = true):(bIE = false);
        textbox.focus();
        
        if (bIE) 
        {
            myRange = textbox.document.selection.createRange();
            oldText = myRange.text;
            newText = GetWikiText(oldText,formatString);
            myRange.text = newText;
            myRange.select;            
        }
        else if (textbox.selectionStart) //Firefox    
        {   
            var iStart, iEnd, iScrollTop;
            iScrollTop = textbox.scrollTop;             
            iStart = textbox.selectionStart;
            iEnd = textbox.selectionEnd;        
            oldText = textbox.value.substring(iStart, iEnd);
            newText = GetWikiText(oldText,formatString);            
            iLen = textbox.selectionEnd - textbox.selectionStart;
            textbox.value = textbox.value.substring(0,iStart) + 
                newText + textbox.value.substring(iEnd,textbox.value.length);   
            textbox.scrollTop = iScrollTop;
        }
        //else do nothing
        textbox.focus;                               
    }           
    
    function GetWikiText(sOld, formatString)
    {
        var sNew;
        switch(formatString)
            {
                case 'italics':  
                    sNew = "''" + sOld + "''";
                    break;
                case 'underline': 
                    sNew = "__" + sOld + "__";
                    break;
                case 'bold': 
                    sNew = "**" + sOld + "**";
                    break;
                case 'strong':
                    sNew = "'''" + sOld + "'''";
                    break;
                case 'center':
                    sNew = "=" + sOld + "=";
                    break;
                case 'h1':
                    sNew = "! " + sOld;
                    break;                
                case 'h2':
                    sNew = "!! " + sOld;
                    break;  
                case 'h3':
                    sNew = "!!! " + sOld;
                    break;
                case 'hr':
                    sNew = "----";
                    break;                    
                case 'ul':
                    sNew = "+" + sOld;
                    break;
                case 'ol':
                    sNew = "#" + sOld;
                    break;
                case 'namedlink':
                    sNew = "{" + sOld + "|http://someurl}";
                    break;
                case 'wikilink':
                    sNew = "[" + sOld + "]";
                    break; 
                case 'custom':
                    sNew = "<<tag>>" + sOld + "<</tag>>";
                    break;                                                                                          
                default:  
                    sNew = sOld;
                    break;
            }
        return sNew;
    }                 