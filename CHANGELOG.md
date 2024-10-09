## 0.1.10
- Updated icon
- Increased the value range for yapping from 10 thousand to 100 thousand.
- Setting yap chance below 10 (0.01%) now disables yapping.
- Decreased default yap chance from 0.05% to 0.03%
- Fixed loiter penalty not resetting between stages
- Reworked loitering penalty
## 0.1.9
- Enemies now have a chance to speak. This is infrequent and can be disabled if desired.
- Fixed typo in data member. ProperSaves may be lost with this change.
## 0.1.8
- Fixed max enemy levels not being reset at the end of a run.
- Level cap change is now listed on the difficulty card.
- ChunkyMode options are now pulled from the host.
## 0.1.7
- Added support for ProperSaves
## 0.1.6
- Nerfed some modded enemies to prevent unfair deaths.
- Added language tokens for Risk of Options settings.
## 0.1.5
- Loiter timer now applies in swarms mode
- Enemy attack speed buff is now listed in the difficulty description
- Changed the way that Acrid's healing buff and the Shield Recharge Rate penalty are calculated
- Moved text strings into a language file
- Added support for Risk of Options
  - All unlisted changes can be disabled if desired (such as the soft cap for computers that struggle to run at a decent framerate with the additional enemies)
  - As of right now there are no plans to make listed changes toggleable
## 0.1.4
- added director dependency to manifest.json
## 0.1.3
- Fixed HP being tripled after 5 minutes when it was supposed to be doubled
## 0.1.2
- Added loitering penalty
- Buffed Vicious Wounds and Ravenous Bite to be unaffected by ally healing
## 0.1.1
- Changed the way rex buffs are hooked
## 0.1.0
- First release