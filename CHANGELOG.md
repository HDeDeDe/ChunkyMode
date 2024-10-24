## 0.2.7
- Loiter penalty is now explicitly disabled in Simulacrum.
- -10% gold penalty no longer applies in Simulacrum.
- -50% ally healing is now only active during a wave in Simulacrum.
- Enemy limit boost now applies to the enemy cap in Simulacrum.
- Most IL hooks should no longer explode.
## 0.2.6
- Updated minimum version of HealthComponentAPI
- Fixed game breaking issue with ProperSaves on non ChunkyMode difficulties.
## 0.2.5
- RiskUI support (lmao)
- Experimental Curse Penalty is more responsive now
- Experimental Curse Penalty now causes loiter punishment to become more intense the more defiant gouges you have.
- Experimental Curse Penalty Rate defaults to 0.035 instead of 0.025
- Moved Experimental Curse Penalty options from Curse to Experiments
- Added Experimental Blind Pest limit, can be turned on in the mod options.
- Decreased minimum Loiter Penalty time from 60 seconds to 15 seconds.
- Fixed a bug where Curse Penalty would not reset if you left a stage with a teleporter without activating the teleporter
## 0.2.4
- Enemy yapping now includes elite affixes.
- Enemies now have a few more lines they can yap.
- Converted run info to sync with UNetWeaver instead of INetRequest.
- Fixed mod option descriptions not being localized.
- Improved error logging with included pdb
- Added experimental curse penalty. It can be enabled in the settings.
## 0.2.3
- Added +50% Ally Barrier Decay Rate
- Nerfed Bastard Pest
- Buffed Bison
- The healing and shield nerfs now use HealthComponentAPI to achieve their effects
## 0.2.2
- Optimized enemy buffs
- Fixed the nerf to Nemesis
## 0.2.1
- Changed loitering penalty to be less insane.
- Loitering penalty severity can now be changed.
- Reduced default loitering penalty frequency from 7 seconds to 5 seconds
- Added warning for the loitering penalty.
- Enemies are twice as likely to yap once the loitering penalty has kicked in.
## 0.2.0
- Updated icon
- Fixed difficulty not appearing in logbook. As a consequence, Chunky Mode no longer has a space.
- Increased the value range for yapping from 10 thousand to 100 thousand.
- Setting yap chance below 10 (0.01%) now disables yapping.
- Decreased default yap chance from 0.05% to 0.03%
- Fixed loiter penalty not resetting between stages
- Added options to tune loiter penalty
- Reworked loitering penalty
  - Penalty now causes the Combat Director to go into overdrive instead of enabling Artifact of Swarms.
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