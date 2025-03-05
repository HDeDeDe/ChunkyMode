## 0.4.11
- Fixed Chef's Kiss not being hooked due to the latest update.
## 0.4.10
- Fixed a bug that prevented the game from starting when Downpour is installed
## 0.4.9
- Fixed the space not appearing in Chunky Mode on the character select page.
- Changed the key ProperSaves uses to load run info, older saves should not be affected.
## 0.4.8
- Fixed Chef's Kiss buffs being broken by the new update
- Updated minimum version for a few dependencies
- Modded survivor buffs will likely be broken for now while those mods are being updated.
## 0.4.7
- Fixed a bug where vanilla survivors after Acrid were not loading their stored buffs when using ProperSaves
- Buffed Paladin to only receive +29% Barrier Decay Rate (If you were below the threshold for Bulwark's Blessing, Sacred Sunlight would cause audio spam due to not applying barrier fast enough)
- Added an option to control how much to subtract from barrier decay rate when playing as Paladin
- The space has returned to Chunky Mode! Enjoy seeing Chunky Mode instead of ChunkyMode once again! This time without broken logbooks!
## 0.4.6
- Update minimum version of Interloping Artifact
## 0.4.5
- Seeker's Unseen Hand and Meditate have been buffed to -25% healing
- Chef's Chef's Kiss has been buffed to -0% healing (dear lord he needs it)
- Ravager's consume has been nerfed to -25% healing
- Added options to control all base game survivor healing.
## 0.4.4
- Fixed a bug that caused resetting options to only work on the title screen
## 0.4.3
- Changed minimum version of HealthComponentAPI
- Due to the HealthComponentAPI update, Barrier Decay rate has been nerfed to +100% to bring it more in line with what it was before.
- Added "reset to default" buttons on every page
## 0.4.2
- Buffed Alien Hominid's Chomp
- Buffed Ravager's consume
- Buffed Submariner's N'kuhanna's Restoration
- Added options to alter survivor buffs (even into nerfs)
- Changed Enemy Yap Chance to be a float instead of an int
- Changed how some values are displayed in options
- Config options are now clamped
- Default values are now listed in options.
- Reduced the number of things being networked.
## 0.4.1
- Forgot to add Interloping Artifact as a dependency
## 0.4.0
- Added Interloping Artifact as a dependency
- Removed options relating to loiter penalty, as these are now handled by Interloping Artifact.
- Loiter penalty is no longer optional, and as such now listed on the difficulty card as `Time Until Aggressive Spawns`.
- Limit Blind Pest option has been moved to the simulacrum category, as the loiter version is now handled by Interloping Artifact.
- Options controlling the loiter penalty can no longer be changed, unless using Artifact of Interloping.
- Experimental Curse Penalty has been removed, but its effect can still be enabled by using Artifact of Interloping.
- Added Scrollable Lobby UI as a dependency
- (Starstorm 2) Buffed Chirr's Secondary
## 0.3.1
- Improved RiskUI support (LMAO)
- Updated description for `Time until loiter penalty` to accurately reflect minimum settings
- Moved Limit Blind Pest and Blind Pest Amount from Experiments to Loitering
- Limit Blind Pest is enabled by default
- Fixed a bug where ChunkyMode claimed to fail to load from ProperSave regardless of if it actually did.
## 0.3.0
- Changed Plugin GUID from `HDeDeDe.ChunkyMode` to `com.HDeDeDe.ChunkyMode`, configs will be reset due to this and any
mods checking for the original may break.
- Experiment options are now in their own config file
- Loiter penalty is now explicitly disabled in Simulacrum.
- -10% gold penalty no longer applies in Simulacrum.
- -50% ally healing is now only active during a wave in Simulacrum.
- Enemy limit boost now applies to the enemy cap in Simulacrum.
- Experimental Pest Limit now applies in Simulacrum.
- Experimental Pest Limit now applies to Lemurians in Simulacrum.
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