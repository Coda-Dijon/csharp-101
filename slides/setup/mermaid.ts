import { defineMermaidSetup } from '@slidev/types'

// Diagrammes aux couleurs du thème : boîtes blanches, bordure rouge sang, flèches noires
export default defineMermaidSetup(() => ({
  theme: 'base',
  themeVariables: {
    fontFamily: 'Inter, sans-serif',
    primaryColor: '#ffffff',
    primaryTextColor: '#141414',
    primaryBorderColor: '#be1e2d',
    lineColor: '#141414',
    edgeLabelBackground: '#ffffff',
  },
}))
