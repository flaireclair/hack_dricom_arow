   
        struct Input {
            float2 uv_MainTex;
            float2 heightRate;
            float3 worldPos;
        };

           void VertFunction (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            
            // 高さの色表現
            if(_IsVisibleColorHeight)
            {
                // 高さ表現は現状、頂点の高さ（ワールド座標の高さではない
                o.heightRate.x = (v.vertex.y - _MinHeight) / (_MaxHeight - _MinHeight);
                
                o.heightRate.x = min(o.heightRate.x,1);
                o.heightRate.x = max(0,o.heightRate.x);
                o.heightRate.y = 0.5f;
            }
        }

        
#if Lighting_OFF 
        void surf (Input IN, inout SurfaceOutputStandard o) 
#else
        void surf (Input IN, inout SurfaceOutput o) 
#endif
        {
            // 等高線を表示させる？（あまり出来はよくない）
            if(_IsVisibleContourLine)
            {
                float height = (sin(IN.worldPos.y/_LoopHeightContourLine) - 1);
                if( 0 < height && height* height < 0.000000000000001 )
                {
                    // 対象であれば、黒くして終了.
                    o.Albedo = float3(0,0,0);
                    o.Alpha = 1;
                    return;
                }
            }
            
            float3 color = _Color;
            
            // 高さの色表現
            color = _IsVisibleColorHeight ? tex2D (_HeightColorTex, IN.heightRate).rgb : color;                
            
            // 道部分の色を重ねる（というか、黒く色を抜く）
            // 今後、本当に黒く色を抜くだけなら
            // color = color - (revColor * 0.8) * _IsRoadTex;
            // と、してしまう方が良さげ
            float3 revColor;
            float a = tex2D (_MainTex, IN.uv_MainTex).a;
            revColor = tex2D (_MainTex, IN.uv_MainTex).rgb;                
            color = _IsRoadTex ?( a > 0.7 ? revColor : color ) : color;
                
            o.Albedo = color; 
            o.Alpha = 1;
        }     

#if Lighting_ON
        fixed4 LightingToonRamp (SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            half d = dot(s.Normal, lightDir)*0.5;
            fixed3 ramp = tex2D(_RampTex, fixed2(d, 0.5)).rgb;
            fixed4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * ramp;
            c.a = s.Alpha;
            return c;
        }
#endif
