# Smith Yourself Project Rules

This document outlines the guidelines for adding new features and refactoring existing code in the **Smith Yourself** project. These rules ensure consistency, maintainability, and readability throughout the codebase.

---

## Adding New Features

### General Guidelines
1. **Modularity**: Always keep components modular and reusable. Encapsulate new functionality in dedicated classes or utility functions rather than adding code directly to `ModEntry.cs`.
2. **Configuration Flexibility**: Expose all new configurable options through `GenericModConfigMenu` (GMCM) so end-users can customize behavior without editing code.
3. **Internationalization**: Add translation strings to the `i18n/default.json` file and reference them via the translation helper. Test with multiple languages when possible.
4. **Android/Mobile Compatibility**: Test new features on Android. Handle touch input in `AndroidControls.cs` if the feature involves player interaction. Avoid UI elements that require precise mouse clicks.
5. **Documentation**: Update relevant documentation files (`README.md`, etc.) to clearly describe new features and their usage.
6. **Testing**: Perform thorough gameplay testing:
   - Test the feature in-game with different configuration options
   - Verify it works on both desktop and Android platforms
   - Test edge cases (e.g., inventory full, invalid items, rapid clicks)
   - Ensure no console errors appear in SMAPI logs

### Specific Guidelines

#### 1. **Adding New Configurable Options**
- **Step 1**: Define the new configuration property in `config/ModConfig.cs` with a sensible default value.
- **Step 2**: Add the option to the GMCM menu in `mod_menu/ModMenu.cs`.
- **Step 3**: Use the option in your feature code via the `ModEntry.Config` static property.
- **Step 4**: Add translation strings to `i18n/default.json` for the option name and description.

**Example: Adding a New Tool Upgrade Speed Option**
```csharp
// Step 1: config/ModConfig.cs
public class ModConfig
{
    // ... existing config options ...
    public int UpgradeSpeedMultiplier { get; set; } = 2;
}

// Step 2: mod_menu/ModMenu.cs (within the BuildMenu method)
gmcm.AddNumberOption(
    name: () => helper.Translation.Get("config.upgrade_speed"),
    tooltip: () => helper.Translation.Get("config.upgrade_speed.description"),
    getValue: () => config.UpgradeSpeedMultiplier,
    setValue: value => config.UpgradeSpeedMultiplier = value,
    min: 1,
    max: 5
);

// Step 3: Use in your feature code
float speed = ModEntry.Config.UpgradeSpeedMultiplier * 0.15f;

// Step 4: i18n/default.json
{
    "config.upgrade_speed": "Upgrade Speed Multiplier",
    "config.upgrade_speed.description": "How fast the upgrade bar fills during the minigame"
}
```

#### 2. **Adding New Assets**
- **Step 1**: Create a new PNG image file and add it to the appropriate subfolder in `assets/`.
- **Step 2**: Load the asset in `mod_utils/Initialization.cs` within the `LoadAssets()` method.
- **Step 3**: Store it in the `SmithingTextures` dictionary with a descriptive key for easy access.
- **Step 4**: Use the texture in your feature code via `ModEntry.init.SmithingTextures["key"]`.

**Example: Adding a New Texture for Smithing Minigame**
```csharp
// Step 1: Add new texture file to assets/minigame_new.png

// Step 2 & 3: mod_utils/Initialization.cs (within the LoadAssets method)
SmithingTextures["minigame_new"] = Helper.Content.Load<Texture2D>("assets/minigame_new.png");

// Step 4: Use in your feature code
Texture2D? texture = ModEntry.init.SmithingTextures["minigame_new"];
if (texture != null)
{
    // Draw texture...
}
```

#### 3. **Adding New Event Handlers**
- **Step 1**: Create a private method in `ModEntry.cs` with the appropriate event signature.
- **Step 2**: Subscribe to the event in the `Entry()` method.
- **Step 3**: Add error handling with try-catch blocks and log exceptions using `Monitor.Log()`.
- **Step 4**: Include comments explaining what the handler does.

**Example: Adding a Handler for Player Level Up Events**
```csharp
// Step 1: ModEntry.cs (new private method)
private void OnPlayerLevelUp(object? sender, LevelChangedEventArgs e)
{
    try
    {
        // Only handle skill level changes, not professions
        if (e.NewLevel > e.OldLevel)
        {
            Monitor.Log($"Player skill {e.Skill} increased to level {e.NewLevel}", LogLevel.Debug);
        }
    }
    catch (Exception ex)
    {
        Monitor.Log($"Error in OnPlayerLevelUp: {ex.Message}", LogLevel.Error);
    }
}

// Step 2: ModEntry.cs (within the Entry method)
Helper.Events.Player.LevelChanged += OnPlayerLevelUp;
```

---

## Refactoring Existing Code

### General Guidelines
1. **Code Clarity**: Make code more readable and understandable without changing its functionality or behavior.
2. **Performance Awareness**: The minigame runs every frame, so be cautious with changes to `StrengthMinigame.cs` and rendering code. Avoid unnecessary allocations or loops.
3. **Backward Compatibility**: When removing features or changing configuration structure, maintain support for old config values or provide clear migration guidance.
4. **Testing**: After refactoring, test gameplay thoroughly to ensure the mod still works correctly in all scenarios.
5. **Incremental Changes**: Make one logical refactoring at a time rather than bundling multiple changes together.

### Specific Guidelines

#### 1. **Refactoring Event Handlers**
- **Step 1**: Review all event handlers in `ModEntry.cs` to identify unused or redundant ones.
- **Step 2**: Remove the handler method and its subscription in the `Entry()` method.
- **Step 3**: Verify that no other code depends on the removed functionality by searching the codebase.
- **Step 4**: Test gameplay to ensure nothing breaks.

**Example: Removing Unused Button Press Handler**
```csharp
// Step 1: Identify unused handler in ModEntry.cs
// This handler was meant for a feature that no longer exists

// Step 2: Remove the handler method
// Delete: private void OnButtonPressUnused(object? sender, ButtonPressedEventArgs e) { ... }

// And remove the registration
// Delete: Helper.Events.Input.ButtonPressed -= OnButtonPressUnused;

// Step 3: Verify no code references this handler
// Use IDE search to confirm no other file calls this method

// Step 4: Test the mod in-game
```

#### 2. **Refactoring Utility Functions**
- **Step 1**: Identify utility functions in `mod_utils/UtilitiesClass.cs` that have similar logic and can be generalized.
- **Step 2**: Create a single, more general method that handles all cases using parameters.
- **Step 3**: Update all call sites to use the new method.
- **Step 4**: Add clear comments explaining what each parameter does.
- **Step 5**: Test edge cases thoroughly.

**Example: Combining Similar Upgrade Helper Methods**
```csharp
// Step 1: Identify similar methods
// Old code had: UpgradeAxe(), UpgradePickaxe(), UpgradeHoe() with nearly identical logic

// Step 2 & 4: Create one general method with comments
public bool UpgradeItem(Item item, ToolType toolType)
{
    // Upgrades the specified item based on tool type
    // Returns true if upgrade was successful, false otherwise
    if (item == null) return false;
    
    // ... use toolType to determine upgrade items and amounts from config
}

// Step 3: Update all call sites
// Old: UpgradeAxe(item);
// New: UpgradeItem(item, ToolType.Axe);

// Step 5: Test that Axe, Pickaxe, and Hoe upgrades all still work
```

#### 3. **Refactoring Configuration Handling**
- **Step 1**: Review `config/ModConfig.cs` and identify unused or redundant properties.
- **Step 2**: Check if any old configuration options are no longer referenced in the codebase.
- **Step 3**: Remove unused properties from the config class.
- **Step 4**: If removing a property that existed in previous versions, document the change in release notes so users know it's safe to delete.
- **Step 5**: Test that the mod loads config correctly and GMCM menu displays all current options.

**Example: Removing Redundant Config Option**
```csharp
// Step 1 & 2: Identify unused property in ModConfig.cs
// Property "ObsoleteOption" was added for a feature that was never completed

// Step 3: Remove from config/ModConfig.cs
public class ModConfig
{
    // ... existing config options ...
    // DELETE: public int ObsoleteOption { get; set; } = 0;
}

// Step 4: Document in release notes
// "Removed unused configuration option: ObsoleteOption"

// Step 5: Test loading the mod with GMCM and verify no errors appear
```

---

## Internationalization (i18n)

Smith Yourself supports multiple languages. Follow these guidelines when adding text to the mod:

### Adding New Translatable Strings
1. **Step 1**: Add the string key and default (English) text to `i18n/default.json`.
2. **Step 2**: Reference the string in code using `Helper.Translation.Get("key.name")` or `Translation.Get()` from the static translation helper.
3. **Step 3**: Provide translations to community translators for other languages (currently supports Korean in `i18n/ko.json`).

**Example: Adding a New Message**
```csharp
// Step 1: i18n/default.json
{
    "message.upgrade_complete": "Your tool has been upgraded!"
}

// Step 2: Use in code
string message = Helper.Translation.Get("message.upgrade_complete");
Monitor.Log(message);
```

### Translation Keys Structure
- Use lowercase with dots for nesting: `config.option_name`, `message.type_name`, `ui.button_label`
- Group related strings together in the JSON for easy maintenance

---

## Android/Mobile Compatibility

Since this mod supports Android devices, consider these guidelines when adding features:

### Mobile-Specific Considerations
1. **Touch Input Handling**: If your feature involves clicking or interaction, ensure it works with touch input. Refer to `mod_utils/AndroidControls.cs` for examples.
2. **UI Sizing**: Mobile screens are smaller. Ensure clickable UI elements are large enough for touch input (minimum ~40x40 pixels).
3. **Avoid Precise Clicking**: Features requiring exact pixel-perfect clicks may frustrate mobile players. Provide larger hit areas when possible.
4. **Test on Android**: Test new features on an Android device or emulator to catch touch input issues early.
5. **Menu Compatibility**: Verify new menus work with mobile input by checking the existing mobile menu implementations.

**Example: Adding Mobile Input Support**
```csharp
// In mod_menu/CustomMenu.cs
public bool HandleMobileInput(ButtonPressedEventArgs e)
{
    // Return true if mobile input was handled
    // Return false if the event should be processed normally
    
    if (Constants.TargetPlatform == GamePlatform.Android)
    {
        // Handle touch position
        Vector2 touchPos = e.Cursor.AbsolutePixels;
        // Check if touch position hits your clickable area
    }
    return false;
}
```

---

## Performance Considerations

The minigame code runs every frame (60 FPS on most systems). Be careful with these areas:

1. **StrengthMinigame.cs**: This is performance-critical. Avoid:
   - Creating new objects in `Update()` or `Draw()` methods
   - Large loops or recursive operations
   - Heavy LINQ queries (`.Where().Select().ToList()`)
   
2. **Rendering**: Minimize draw calls and texture uploads
3. **Event Handlers**: Keep event handler logic fast. Defer heavy processing to idle times using `GameLoop.UpdateTicked` with throttling

**Example: Efficient Event Handler**
```csharp
// AVOID: Heavy processing in ButtonPressed
private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
{
    // DON'T do: ExpensiveCalculation(); 
}

// BETTER: Defer to slower events
private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
{
    if (e.IsOneSecond)  // Run once per second instead of every frame
    {
        ExpensiveCalculation();
    }
}
```

---

## Dependency Management

### Adding External Dependencies
- **NuGet Packages**: Only add NuGet dependencies if absolutely necessary. Check if SMAPI provides the functionality first.
- **Other Mods**: If you need to integrate with another mod, use the mod registry with try-catch to gracefully handle if the mod isn't installed.
- **Document Dependencies**: Update `manifest.json` with any mod dependencies and their minimum versions.

**Example: Using Another Mod's API**
```csharp
// In OnGameLaunched
var otherModApi = Helper.ModRegistry.GetApi<IExampleModApi>("author.ExampleMod");
if (otherModApi != null)
{
    // Use the API
}
else
{
    Monitor.Log("ExampleMod not found, proceeding without integration", LogLevel.Info);
}
```

---

## Conclusion

Adhering to these rules ensures that the **Smith Yourself** project remains maintainable, performant, and accessible to players across different platforms and languages. Always prioritize clarity, test thoroughly, and document your changes.

For any specific questions or issues related to adding features or refactoring code, refer to this document or seek guidance from the project's maintainers.