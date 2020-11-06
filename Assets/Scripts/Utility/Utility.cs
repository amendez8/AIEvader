using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour {

    // Loop trough parents until you find a Gameobject that matches the type
    public static GameObject FindParentWithComponentOfType<T>(ref T type, GameObject childObject)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.GetComponent<T>() != null)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }

	public static void HideMouse(){		
		Cursor.visible = false;
	}

	public void StaticWraperHideMouse(){
		// OnClick events on Unity are real dumb and need an instance object to run a static function
		HideMouse();
	}
}
