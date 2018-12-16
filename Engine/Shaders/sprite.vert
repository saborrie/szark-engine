#version 400 

layout(location = 0) in vec2 pos;
layout(location = 1) in vec2 tex;

out vec2 texCoord;

uniform mat4 projection;
uniform mat4 model;
uniform mat4 rotation;
uniform mat4 scale;

void main() 
{
    texCoord = tex;
    gl_Position = projection * scale * model * rotation * vec4(pos.x, pos.y, 0f, 1.0f);
}