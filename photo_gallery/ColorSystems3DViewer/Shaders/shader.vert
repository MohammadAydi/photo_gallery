#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aColor;
layout(location = 2) in vec3 aChannels;

out vec3 vColor;

uniform mat4 uMVP;
uniform vec3 uRangeMin;
uniform vec3 uRangeMax;
uniform float uPeel;
uniform bool uSubtract;
uniform bool uNoFilter;

void main()
{
    if (!uNoFilter)
    {
        bool inside =
            aChannels.x >= uRangeMin.x && aChannels.x <= uRangeMax.x &&
            aChannels.y >= uRangeMin.y && aChannels.y <= uRangeMax.y &&
            aChannels.z >= uRangeMin.z && aChannels.z <= uRangeMax.z &&
            aChannels.x <= uPeel;

        bool visible = uSubtract ? !inside : inside;

        if (!visible)
        {
            gl_Position = vec4(2.0, 2.0, 2.0, 1.0);
            vColor = vec3(0.0);
            return;
        }
    }

    gl_Position = uMVP * vec4(aPos, 1.0);
    gl_PointSize = 3.0;
    vColor = aColor;
}