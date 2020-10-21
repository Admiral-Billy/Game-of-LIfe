Heya! This project was actually a lot more fun than I thought it would be initially heh.

Anyways, I've taken a few creative liberties with some of the instructions:
1. Randomization isn't done with time, but is done with GUIDs which are more or less just as random (and seemed much easier to work with).
2. There's an option to randomize on any sort of reset, which is enabled by default; turning this off lets you change values and create empty universes with those. The option is available in both the options menu and right click context menu.
3. Saving is done in plaintext format (and file extension .cells like on the LifeWiki) as you mentioned, although I didn't add any ability to add comments.
4. I added two different load options that are applicable for any .cells file! The first simply loads that file as its own universe, with the exact same conditions in the file. The second imports it into the top left corner of the current universe, with an error being shown if the universe isn't large enough to support it.
5. Most of my settings are contained within a singular settings menu instead of the different structures mentioned, although I put the visual settings in a separate customize menu.
6. I used the built-in ColorDialog structure for the purposes of the color menu.
7. The persistent settings are implemented a bit differently than the rubric says; I save *all* settings except for the universe itself and there is a button to save your settings instead of automatically saving on exit; so the options menu has "Apply" (save settings for this session), "Save settings" (for persistent saving), "Load last settings" (To load whatever is contained in that file), and "Reset to default" (self explanatory, but doesn't overwrite a file unless you save afterwards).

So yeah, I believe I at least have ticked off every checkbox on the rubric, but some things are implemented differently and some things have extra features. I didn't take the "easy" way out for any of those on purpose though; if I implemented something differently, it's because I truly thought that worked best/was best for the user. I've more or less commented everything so it should be easy enough to tell what things are doing, and organization is a basic "most important -> least important" sort.

Let me know if any of these features fall flat compared to the rubric though! Even if it isn't graded, I'd like to try and correct them even if it's after the due date.