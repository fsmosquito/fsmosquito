import { IconProp } from '@fortawesome/fontawesome-svg-core';

import {
  faAngleDoubleRight,
  faGlobeAmericas,
  faStream,
  faInventory,
  faTools,
  faShieldCheck,
  faStore,
  faHeadphonesAlt,
  faJoystick,
  faSyncAlt,
  faWifi,
} from '@fortawesome/pro-duotone-svg-icons';

export interface NavItemProps {
  id: string;
  messageId: string;
  title: string;
  icon?: IconProp;
  url?: string;
  type?: string;
  count?: number;
  color?: string;
  children?: NavItemProps[];
}

const routesConfig: NavItemProps[] = [
  {
    id: 'automation_studio',
    title: 'Automation Studio',
    messageId: 'sidebar.automation_studio',
    icon: faSyncAlt,
    color: 'blue',
    url: '/automationstudio',
  },
  {
    id: 'app',
    title: 'Applications',
    messageId: 'sidebar.applications',
    icon: faAngleDoubleRight,
    color: 'blue',
    children: [
      {
        id: 'sound_board',
        title: 'Sound Board',
        messageId: 'app.sound_board',
        icon: faHeadphonesAlt,
        url: '/apps/soundboard',
      },
      {
        id: 'moving_map',
        title: 'Moving Map',
        messageId: 'sidebar.app.moving_map',
        icon: faGlobeAmericas,
        url: '/apps/movingmap',
      },
      {
        id: 'marketplace',
        title: 'Marketplace',
        messageId: 'sidebar.app.marketplace',
        icon: faStore,
        url: '/apps/marketplace',
      },
      {
        id: 'logbook',
        title: 'Logbook',
        messageId: 'sidebar.app.logbook',
        icon: faStream,
        url: '/apps/logbook',
      },
      {
        id: 'badges',
        title: 'Badges',
        messageId: 'sidebar.app.badges',
        icon: faShieldCheck,
        url: '/apps/badges',
      },
    ],
  },
  {
    id: 'tools',
    title: 'Tools',
    messageId: 'nav.tools',
    icon: faTools,
    children: [
      {
        id: 'status',
        title: 'Status',
        messageId: 'nav.tools.status',
        icon: faWifi,
        url: '/tools/status',
      },
      {
        id: 'task_manger',
        title: 'Task Manager',
        messageId: 'nav.tools.task_manager',
        icon: faInventory,
        url: '/taskmanager',
      },
      {
        id: 'virtual_controls',
        title: 'Virtual Controls',
        messageId: 'nav.tools.virtual_controls',
        icon: faJoystick,
        url: '/tools/virtualcontrols',
      },
    ],
  },
];
export default routesConfig;
