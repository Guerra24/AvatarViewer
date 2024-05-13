ARG GAMECI_IMAGE=unityci/editor:ubuntu-6000.0.1f1-windows-mono-3
FROM $GAMECI_IMAGE

ARG BLENDER_SHORT_VERSION=3.5
ARG BLENDER_FULL_VERSION=3.5.1

RUN wget -q https://download.blender.org/release/Blender$BLENDER_SHORT_VERSION/blender-$BLENDER_FULL_VERSION-linux-x64.tar.xz && \
    tar -xf blender-$BLENDER_FULL_VERSION-linux-x64.tar.xz && \
    rm blender-$BLENDER_FULL_VERSION-linux-x64.tar.xz

ENV PATH="$PATH:/blender-$BLENDER_FULL_VERSION-linux-x64"
