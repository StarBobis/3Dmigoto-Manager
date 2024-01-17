# Open source is a kind of sin
No one will thanks you for open source these scripts, they just use your scripts and at the same time verbally attacking you,when i open source reverse scripts seems everyone want to insult me to show their fake ethical, so my project will never be open source again, may be some compiled exe will be published here but never source code again.

# Usage notice
3DmigotoManager and all it's content is educational purpose only, please avoid abuse.

# 3DmigotoManager
Manage multiple game's 3dmigoto and increase mod author's effeciency.

It's a very simple tool but solved my problem for using 3dmigoto on multiple game 
but very cumbersome to maintain their file and icons, now we have a unified way.

Notice: the 3Dmigoto-Knife inside this project is forked and modified from original 3Dmigoto's injector but add some extra features,but I never release it in new version because microsoft always think it's a virus dut to inject way.

the 3Dmigoto-Armor version of d3d11.dll is forked and modified from original 3Dmigoto repository and can be found at here: https://github.com/StarBobis/3Dmigoto-Armor-23-12-24-fork ,but in V1.2.0 and later version, I add a mod encryption method in source code and lead the repository becomes private again, 
now the open source version of Armor is enough for educational purporse so I archived it.

3Dmigoto-Armor is a modified version for game mod and it's open source instead of GIMI's d3d11.dll is a closed source version, but two of them have nearly same functions.
but mod encryption version is private and due to encryption algorithm I can't share the source code, sorry.

![image](https://github.com/StarBobis/3DmigotoManager/assets/151726114/4224c3cf-413d-4666-a53b-6d6103468f60)
# 3Dmigoto-Armor core features
- Dynamic d3d11 desc's byte width increase changed with your mod's vertex number,which will save a lot of memory and avoid memory leak and avoid game crush,it's better than GIMI's d3d11.dll's solid d3d11 desc byte width increase number in some special game like NBP.
- Mod format encryption.

# Features
- Supported game list:GI, HI3, HSR, ZZZ, DSP, KALABIYAU, YSLZM
Special type: GIMI_DEV,GIMI_PLAY,SRMI_DEV,SRMI_PLAY 
These Special type loader is copied from GIMI and SRMI's release because so many people are using them so this software will support them specially.
but in V1.2.0 version and later all d3d11.dll is using armor version instead of d3d11.dll copied from original repository,
if you want to enable GIMI feature and do not want to use Armor feature, you can switch it manually.
- Auto FrameAnalysis Folder clean.
- Auto run 3dmigoto and run game.
- Multiple game's unified 3dmigoto folder management.

# How to improve this project?
- Open an issue to send feedback and advice.
- Join 3MA server to send me feedback directly:https://discord.gg/JEcWVKr7wu

# Thanks for 3Dmigoto
Without their original 3dmigoto repo the game mod version will be impossible. Huge thanks to Chiri,DarkStarSword,bo3b and 3Dmigoto original author group.

Special thanks for GIMI developer: SilentNightSound
