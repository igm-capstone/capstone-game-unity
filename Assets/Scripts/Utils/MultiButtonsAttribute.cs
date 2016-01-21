using System.Deployment.Internal;
using UnityEngine;

public class MultiButtonsAttribute : PropertyAttribute
{
    public MultiButtonsAttribute() : base()
    {
        Columns = 1;
    }
    public int Columns { get; set; }
}
