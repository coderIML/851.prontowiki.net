var XMLHTTP = false;

try {
  XMLHTTP = new ActiveXObject("Msxml2.xmlHttp");
} catch (e) {
  try {
    XMLHTTP = new ActiveXObject("Microsoft.xmlHttp");
  } catch (ex) {
    XMLHTTP = false;
  }
}

if (!XMLHTTP && typeof(XMLHttpRequest)!= 'undefined') {
  XMLHTTP = new XMLHttpRequest();  
}

function PostRequest(url, requestString) 
{
    try{      
      XMLHTTP.open("POST", url, true);
      XMLHTTP.setRequestHeader('Content-Type','application/x-www-form-urlencoded');
      XMLHTTP.onreadystatechange = OnRequestStateChange;
      XMLHTTP.send(requestString);
    } catch(e) {
        //alert(e);
    }
}

function GetRequest(url)
{
    try{      
      XMLHTTP.open("GET", url, true);      
      XMLHTTP.onreadystatechange = OnRequestStateChange;
      XMLHTTP.send();
    } catch(e) {
        alert(e);
    }
}

function OnRequestStateChange()
{    
    if ( (XMLHTTP.readyState == 4) && (XMLHTTP.status == 200) ) {        
        ParseTagResponse(XMLHTTP.responseText);        
    }
}

function ParseTagResponse(response)
{
    //poor man's xml parsing for the response
    if (response.length > 0) {
        switch (response.substring(0,5)) {
            case "<taga" :
                GetTags(GetPageName());
                break;
            case "<tagl" :
                var tagHolder = document.getElementById('divTagList');
                var tagArray = (response.substring(9,response.length - 10)).split(";");
                if (tagArray.length > 0) {
                    var result = "";
                    for (var i = 0; i < tagArray.length - 1; i++) {
                        result += '<a href="tags.aspx?t=' + escape(tagArray[i]) + '">' + tagArray[i] + '</a>,&nbsp;';  
                    }
                    //get rid of the trailing comma
                    result = result.substring(0,result.length - 7);
                    tagHolder.innerHTML = result;
                }                      
                                  
                break;
            case "<tagd" :
                GetTags(GetPageName());
                break;
            default:
                alert("an error occurred: " + response);
                break;
        }
    }
}

/**********************************************************************************************************************
* Page Level functions
**********************************************************************************************************************/

function GetPageName()
{
    return document.getElementById('ctl00_ContentPlaceHolder1_lblHeader').innerHTML;
}

function GetTags(pageName)
{
   var request;
   request = "Action=GetAllTags&pageName=" + escape(pageName);
   PostRequest("taghandler.aspx",request);   
}

function AddTag(tag, pageName)
{ 
   if (tag == "") return;   
   var request;
   request = "Action=AddTag&pageName=" + escape(pageName) + "&tag=" + escape(tag);
   PostRequest("taghandler.aspx",request);
}

function DeleteTag(tag, pageName)
{
   var request;
   request = "Action=DeleteTag&pageName=" + escape(pageName) + "&tag=" + escape(tag);
   PostRequest("taghandler.aspx",request);
}

function ToggleTagVisibility()
{
    var divTags;
    divTags = document.getElementById('divTags');
    if (divTags.style.display == 'inline') {
        document.getElementById('toggleTags').innerHTML = 'tags&gt;&gt;';
        divTags.style.display = 'none';
    } else {
        GetTags(GetPageName());
        document.getElementById('toggleTags').innerHTML = 'tags&lt;&lt;';
        divTags.style.display = 'inline';
        
    }
        
}    