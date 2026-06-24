<h2 align="center">A mod for From the Depths that freshens up the Breadboard UX</h2>
<p align="center">
<img width="480" height="300" alt="Compares modified and unmodified UI with a sweeping wipe transition" src="https://github.com/user-attachments/assets/885671b3-f333-4ebe-a3e1-46a3e090fa9a" />  
</p>
<hr/>
Cleans up the design and adds quality-of-life

### -=Changelog=-
#### v1.0.0 - 2026-04-09
* Much less wasted space across the UI
* Hotkey to re-open last opened bread (ported from [BreadThing](https://steamcommunity.com/sharedfiles/filedetails/?id=3540650411))
* Default grid size lowered to 25

#### v1.1.0 - 2026-04-19
Adds keyboard shurtcuts to quick-add components  
Exhaustive list: https://pastebin.com/sMDUNQth  
Alt + [Letter] OR Alt + [Letter][Letter]  
Depending on if it had to share a letter or not.  
 
Examples:
Alt + R places a Rotation component
Alt + G -> S without releasing alt inbetween places a Generic block Setter 

#### v1.2.0 - 2026-06-10
* Adds ability to recolor all selected components  
#### v1.2.1 - 2026-06-14
* Now dynamically reserves space for buttons above main circuit box instead of being hardcoded  
#### v1.2.2 - 2026-06-17
* Simplified change to Prefab Load UI, as a result it now scales more gracefully with window width  

#### v1.3.0 - 2026-06-24  
* Added customizable component layout, 4 included presets (Basegame, FreshBread, BreadTing-ish, Wiki Grouping), selectable in-game in new settings area  
* Allow comment text input to scale with text  
* Allow copy-paste components across breadboards  
* Hide ID on Variable Reader/Writer to reduce size (can be toggled back on)  
* Allow Missile breadboards to also be re-opened with hotkey  
* Stop GBSetter from overflwing the window
