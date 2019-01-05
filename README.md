This repo is intended to contain instructions and resources to set up a build server for Unity using Team City and Docker.


CHANGES TO ADD? 
Add /data/builds volume to the tc server?



# This guide assumes you have a good enough understanding of git to already be using GitHub or GitLab to manage your repo

PROJECT SET UP
# Before you set up the server, you need to do the following:
- Ensure your git repo is a Unity project
- Add the AutomatedBuilder.cs script (in the build-scripts folder of this repo) to your project in Unity in the Assets/Editor director, and commit it to master.
- Update the list of scenes in your build in the AutomatedBuilder.cs script. (The line with "buildPlayerOptions.scenes" in it)
- Change PROJECT_NAME to your project name. When you have multiple projects it is important that these names don't overlap.  



SERVER SET UP

Install Ubuntu Server or Desktop

sudo apt-get update
sudo apt install docker.io ssh python -y

# Add your user to the docker group, then log out and log back in. This makes it so you don't have to use sudo for everything.
sudo usermod -a -G docker $USER


# Pull the teamcity-server image (https://hub.docker.com/r/jetbrains/teamcity-server/)
docker pull jetbrains/teamcity-server

# Make the directories for the server to use (If you change these paths, change them in the run command as well)
mkdir teamcity
mkdir teamcity/data
mkdir teamcity/logs
mkdir teamcity/builds

# Run the server
docker run -d --name teamcity-server-instance  \
    -v ~/teamcity/data:/data/teamcity_server/datadir \
    -v ~/teamcity/logs:/opt/teamcity/logs  \
    -p 8111:8111 \
    jetbrains/teamcity-server


# Go to the server in it's WebUI (If you don't know the ip of your server run 'ifconfig')
# Example would be http://192.168.0.20:8111

Click "Proceed"
Leave "Internal (HSQLDB)" selected and click "Proceed"
After a pause it will ask you to accept the license agreement. Agree and Continue.
Create an admin account following on screen prompts.
	(Optional) - Fill out the name and email on the page, configure your time zone, etc.


SSH Key Configuration 
	- If your repo is private and you use MFA on your Git acccount, you need to use SSH keys, otherwise it's an optional alternative to using your username and password for Git
	- On your server run the following commands (you will be copying and pasting the output of the last two):
		ssh-keygen -f ~/.ssh/tc-key -t rsa -N ''
	- There are lots of ways you could get this key off the box, if you have no idea how, this is one quick way.
		cd ~/.ssh
		python -m SimpleHTTPServer 8000
		# visit "<server ip>:8000" in a browser and click both tc-key and tc-key.pub to download them
		# hit "control + c" one or two times to stop the server (note: under no circumstances ever would it be a good idea to leave that running exposing your keys)

	- In the Team City WebUI go to Administration > Projects, and click "<Root project>" 
	- In the left panel select "SSH Keys" (If it's not visible, click "show more")
	- Click the "Upload SSH Key" button
	- Give the key a name like "unity-tc-server" or something
	- Click "Choose File" and navigate to "tc-key" (NOT tc-key.pub)

	# If you are using GitLab
	- Click your profile picture in the top right, then Settings
	- In the left panel click "SSH Keys"
	- Open "tc-key.pub" in a text editor, and copy + paste the full contents out. (It should start with ssh-rsa and end with <server username>@<server hostname>)
	- Enter a Title like "unity-tc-server" or something and click "Add key"

	# If you are using GitHub
	- Click your profile picture in the top right, then Settings
	- In the left panel click "SSH and GPG Keys", then click "New SSH Key"
	- Open "tc-key.pub" in a text editor, and copy + paste the full contents into the "Key" field. (It should start with ssh-rsa and end with <server username>@<server hostname>)
	- Enter a Title like "unity-tc-server" or something and click "Add SSH key"

Add Your First Project (Repo)
	- Click the "Projects" tab in the top-left corner, then "Create Project"
	- In your repo click the "Clone" button then copy the link that starts with "git@" and paste it in the "Repository URL" field
	- If you did not set up SSH Keys, and the repo is not a public repo, enter your git Username and Password
	- Hit "Proceed", wait for it to come back successfully then hit "Proceed" again.

Add the Unity Plugin (https://plugins.jetbrains.com/plugin/11453-unity-support)
	- Go to the link above and download the latest build (it will be a .zip file)
	- In the Team City WebUI, go to Administration > Plugins List
	- Click "Upload plugin zip", choose the file you just downloaded, and click "Upload plugin zip"
	- Click "Enable uploaded plugins", then "Enable" on the pop-up dialog

Configure the Build Steps
	- Go back to Projects > <Your Project Name> > Edit Project Settings and click the "Edit" button in the Build Configurations section
	- In Artifact Paths, enter the same path in your AutomatedBuilder.cs "buildPlayerOptions.locationPathName" line. (Add multiple paths if you are doing multiple builds -- e.g. uncommenting the Android Build)
	- Click the "Save" button
	- Click "Build Steps" in the left panel
	- Click "Auto-detect build steps"
	- Check the box next to "Unity" and click "Use selected"
	- Click "Edit" to the right of step 1, which is titled "1. Unity"
	- Click the magic wand next to the "Execute method" box, it should auto detect "AutomatedBuild.Build", which you select by clicking 
	- Check "Do not initialize the graphics device"
	- Click the "Save" button

BUILD AND RUN THE AGENT
	# The following commands make the directory, pull the repo down, and begin the building of the agent
	# Note: Building the agent involves downloading and installing Unity, and can take a considerable amount of time
	mkdir ~/git
	cd ~/git
	git clone https://github.com/phearbot/unity-teamcity-docker
	cd unity-teamcity-docker/unity-tc-agent/
	docker build . --tag unity-tc-agent-2018.3:latest
