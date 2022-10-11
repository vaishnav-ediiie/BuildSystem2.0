# Notes
    1. The Grid System and Build System are two different Isolated Plugin. 
    2. Grid System can be used independant to Build System. Build System needs Grid System to work.
    3. If you don't want photon integration then just delete the script "Core/BuildSystemPhotonHandler.cs"

# Setup from Editor
    1. Open the scene you want to use build system in
    2. Create empty object, and add "BuildSystem" component on it
    3. Assign the "Player" & "Player Cam" fields
    4. Other fields can be tuned according to your need, all field have Tooltips.
    5. In the OnEnable method of any script set the "BuildSystem.Brain.AllPlaceableData" parameter
        This must happen in OnEnable method, cause BuildSystem checks this parameter in Start Method.


# Setup from Script
    1. Just call one of two static overloads of the method "BuildSystem.Setup"
    2. This method will return a BuildSystem, store it in a variable.
    3. Set the "BuildSystem.Brain.AllPlaceableData" parameter right after Setup call


# Change the Bindings
    1. You can create a class that inherits from BuildSystemBrain class and override the virtual methods.
    2. Whenever you want to use this new binding for a specific buildSystem instance, just call "buildSystem.UseBrain" method and pass in an Instance of the new class you've created
