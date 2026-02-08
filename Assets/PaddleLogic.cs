using UnityEngine;

public class PaddleLogic : MonoBehaviour
{

    void Update()
    {
        // ---------------------------------------------------------
        // STEP 1: GET INPUT (Screen Coordinates)
        // ---------------------------------------------------------
        
        // 'Vector3' is a struct that holds x, y, z values.
        // 'Input.mousePosition' returns the mouse location in PIXELS (e.g., x: 1920, y: 1080).
        // The Z value here is initially 0 because the screen is 2D.
        Vector3 mousePosScreen = Input.mousePosition;

        // ---------------------------------------------------------
        // STEP 2: CONVERT TO WORLD SPACE
        // ---------------------------------------------------------

        // We modify the 'z' component of the vector.
        // Why? Because 'ScreenToWorldPoint' needs to know "how far from the camera" the object is.
        // If z stays 0, the point is inside the camera lens and becomes invisible.
        // 10f is the standard distance for a 2D Orthographic camera at Z = -10.
        mousePosScreen.z = 10f; 

        // 'Camera.main' finds the first camera tagged as "MainCamera".
        // 'ScreenToWorldPoint' takes the pixel coordinates and mathematically translates them
        // into "World Units" (the coordinates used in the Scene view).
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);

        // ---------------------------------------------------------
        // STEP 3: APPLY MOVEMENT
        // ---------------------------------------------------------

        // 'transform.position' controls where this GameObject is in the world.
        // We create a 'new Vector3' because we want to construct a specific position:
        // - X: Comes from the mouse world position (moves left/right).
        // - Y: Hardcoded to -4f (locks the vertical position so it doesn't move up/down).
        // - Z: 0f (keeps it on the 2D plane).
        transform.position = new Vector3(mousePosWorld.x, -4f, 0f);
    }
}