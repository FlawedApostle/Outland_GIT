using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class VHSFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class VHSSettings
    {
        public Material vhsMaterial;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public VHSSettings settings = new VHSSettings();

    private VHSPass pass;

    public override void Create()
    {
        pass = new VHSPass(settings.vhsMaterial);
        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.vhsMaterial == null)
            return;

        renderer.EnqueuePass(pass);
    }

    class VHSPass : ScriptableRenderPass
    {
        private Material material;

        public VHSPass(Material mat)
        {
            material = mat;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null)
                return;

            // Get camera color buffer (MODERN WAY)
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            TextureHandle source = resourceData.activeColorTexture;

            // Create destination texture
            TextureDesc desc = renderGraph.GetTextureDesc(source);
            desc.name = "VHS_Destination";
            desc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(desc);

            // Blit source → destination using your VHS material
            RenderGraphUtils.BlitMaterialParameters blitParams =
                new RenderGraphUtils.BlitMaterialParameters(
                    source,
                    destination,
                    material,
                    0
                );

            renderGraph.AddBlitPass(blitParams, "VHS Effect Pass");

            // Replace camera color with processed texture
            resourceData.cameraColor = destination;
        }
    }
}
