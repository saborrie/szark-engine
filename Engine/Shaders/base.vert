#version 400 

layout(location = 0) in vec2 pos;
layout(location = 1) in vec2 tex;

out vec2 coords;

void main()
{
    gl_Position = vec4(pos.x, pos.y, 0.0f, 1.0f);
    coords = tex;
}