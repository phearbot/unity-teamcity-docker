FROM jetbrains/teamcity-minimal-agent:latest

# Installing dependencies
RUN apt-get -qq update && apt-get -qq install -y \ 
   curl gconf-service lib32gcc1 lib32stdc++6 libasound2 libc6 libc6-i386 libcairo2 libcap2 libcups2 \
   libdbus-1-3 libexpat1 libfontconfig1 libfreetype6 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libgl1-mesa-glx \
   libglib2.0-0 libglu1-mesa libgtk2.0-0 libnspr4 libnss3 libpango1.0-0 libstdc++6 libx11-6 libxcomposite1 \
   libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxtst6 zlib1g debconf npm \
   xdg-utils lsb-release libpq5 xvfb libsoup2.4-1 libarchive13 libgtk-3-0 unzip \ 
   && rm -rf /var/lib/apt/lists/*
 
# Downloading and setting permissions on the Unity Installer for Linux
RUN curl -o /unity-installer https://beta.unity3d.com/download/6e9a27477296/UnitySetup-2018.3.0f2
RUN chmod +x /unity-installer

# Runs the installer installing Unity, WebGL, iOS, and Android packages. Remove any package from the arg to save space.
RUN yes | /unity-installer --unattended --install-location=/opt/Unity --verbose -c Unity,WebGL,iOS,Android
ADD Unity_lic.ulf /root/.local/share/unity3d/Unity/Unity_lic.ulf

# ANDROID SDK SUPPORT
RUN curl -o /android-sdk.zip https://dl.google.com/android/repository/sdk-tools-linux-4333796.zip
RUN unzip android-sdk.zip -d /opt/android-sdk/
RUN y | /opt/android-sdk/tools/bin/sdkmanager "build-tools;28.0.3"
