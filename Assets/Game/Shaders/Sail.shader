Shader "Custom/Sail" {
	Properties {
		_Color ("Color Tint", Color) = (1,1,1,1) 
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Lighting Off
		CULL Off
		
		Pass
        {
        	SetTexture [_MainTex]
            {
            	ConstantColor [_Color]
            	Combine Texture * constant
        	}
        }
	} 
	FallBack "Diffuse"
}
