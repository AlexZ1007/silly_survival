using UnityEngine;

public class FadeObject : MonoBehaviour
{
    public Material transparentMaterial; // Assign in Inspector

    private Material[] originalMaterials;//store original material to restore
    private Renderer rend;//reference to tha obj renderer
    private bool isFaded = false;//track if the obj is faded

    void Start()
    {
        rend = GetComponent<Renderer>();//get the renderer component
        
        //save the original material
        if (rend != null)
            originalMaterials = rend.materials;
    }

    //makes the obj trasparent 
    public void Fade()
    {
        if (isFaded || rend == null) return;

        Material[] mats = new Material[originalMaterials.Length];
        for (int i = 0; i < mats.Length; i++)
            mats[i] = transparentMaterial;

        rend.materials = mats;
        isFaded = true;
    }

    //restores the original materials
    public void Unfade()
    {
        if (!isFaded || rend == null) return;

        rend.materials = originalMaterials;
        isFaded = false;
    }
}
