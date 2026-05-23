#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aColor;

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
        vec3 norm = (aPos + 1.0) * 0.5;

        bool inside =
            norm.x >= uRangeMin.x && norm.x <= uRangeMax.x &&
            norm.y >= uRangeMin.y && norm.y <= uRangeMax.y &&
            norm.z >= uRangeMin.z && norm.z <= uRangeMax.z &&
            norm.x <= uPeel;

        bool visible = uSubtract ? !inside : inside;

        if (!visible)
        {
            gl_Position = vec4(2.0, 2.0, 2.0, 1.0);
            vColor = vec3(0.0);
            return;
        }
    }

    gl_Position = uMVP * vec4(aPos, 1.0);
    gl_PointSize = 4.0;
    vColor = aColor;
}