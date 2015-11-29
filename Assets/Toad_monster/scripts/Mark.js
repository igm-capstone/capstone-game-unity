#pragma strict


var aTexture : Texture;

function OnGUI() {    

if(!aTexture){        

Debug.LogError("Assign a Texture in the inspector.");        
return;    
}    
 
GUI.DrawTexture(Rect(760,500,80,80), aTexture);

}


