#version 400 

out vec4 color;
in vec2 TexCoords;

uniform sampler2D tex;

void main() {
    color = texture(tex, TexCoords);
}