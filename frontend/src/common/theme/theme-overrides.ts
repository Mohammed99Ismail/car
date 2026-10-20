import type { GlobalThemeOverrides } from 'naive-ui'

const themeOverrides: GlobalThemeOverrides = {
  common: {
    primaryColor: '#00ad4c',
    errorColor: '#FF0055',
    warningColor: '#FF8000',
    borderRadius: '5px',
    borderRadiusSmall: '3px',
    borderColor: '#e4e7ec',
    fontFamily: "'Cairo', 'Inter var', system-ui, -apple-system, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif",
    fontFamilyMono: "ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace",
  },
  Card: {
    borderRadius: '7px',
  },
  Tag: {
    borderRadius: '4px',
  },
  List: {
    borderRadius: '0',
    borderColorPopover: '#e4e7ec',
  },
  Notification: {
    padding: '15px',
  },
  Switch: {
    railColorActive: '#10b981',
    railColor: '#cbd5e1',
    buttonColor: '#ffffff',
  },
}

export const darkThemeOverrides: GlobalThemeOverrides = {
  common: {
    primaryColor: '#00ad4c',
    errorColor: '#FF0055',
    warningColor: '#FF8000',
    borderRadius: '5px',
    borderRadiusSmall: '3px',
    borderColor: '#4b556eff',
    cardColor: '#202c4633',
    popoverColor: '#0f172a',
    modalColor: '#1c202c',
    fontFamily: "'Cairo', 'Inter var', system-ui, -apple-system, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif",
    fontFamilyMono: "ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace",
  },
  Card: {
    borderRadius: '7px',
    color: '#1c202c',
  },
  Dropdown: {
    color: '#1c2334',
  },
  Drawer: {
    color: '#1c202c',
  },
  DataTable: {
    thColor: '#1c202c',
    tdColor: '#1c2334',
    hoverColor: '#1c202c',
    tdColorHover: '#1c202c',
  },
  Tag: {
    borderRadius: '4px',
  },
  List: {
    borderRadius: '0',
    borderColorPopover: '#1c2334',
    colorHoverPopover: '#1c202c',
  },
  Switch: {
    railColorActive: '#10b981',
    railColor: '#334155',
    buttonColor: '#ffffff',
  },
  Menu: {},
}

export default themeOverrides
