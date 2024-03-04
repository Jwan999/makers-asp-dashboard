import {
  faGears,
  faUsers,
  faChalkboard,
  faUserTag,
  faBarsProgress,
  faChartPie,
  faPersonChalkboard,
  faHandshake,
  faPerson,
  faPieChart,
  faGraduationCap,
  faRocket,
  faStar,
  faBoxOpen,
} from "@fortawesome/free-solid-svg-icons";

const navData = [
  {
    title: "Dashboard",
    path: "/dashboard/screens/dashboard",
    icon: faChartPie,
  },
  {
    title: "Partners",
    path: "/dashboard/screens/partners",
    icon: faHandshake,
  },
  {
    title: "Projects",
    path: "/dashboard/screens/projects",
    icon: faBarsProgress,
  },
  {
    title: "Training",
    path: "/dashboard/screens/training",
    icon: faChalkboard,
  },
  {
    title: "Trainers",
    path: "/dashboard/screens/trainers",
    icon: faPersonChalkboard,
  },
  {
    title: "Products",
    path: "/dashboard/screens/products",
    icon: faBoxOpen,
  },
  {
    title: "Services",
    path: "/dashboard/screens/services",
    icon: faBoxOpen,
  },
  {
    title: "Startups",
    path: "/dashboard/screens/startups",
    icon: faRocket,
  },
  {
    title: "Interns",
    path: "/dashboard/screens/interns",
    icon: faPerson,
  },
  {
    title: "Students",
    path: "/dashboard/screens/students",
    icon: faGraduationCap,
  },
  {
    title: "Success Stories",
    path: "/dashboard/screens/successstories",
    icon: faStar,
  },
  {
    title: "Users",
    path: "/dashboard/screens/usermanagement",
    icon: faUsers,
  },
  {
    title: "Roles",
    path: "/dashboard/screens/rolemanagement",
    icon: faUserTag,
  },
  {
    title: "Paramters",
    path: "/dashboard/screens/systemparameters",
    icon: faGears,
  },
  {
    title: "Reports",
    path: "/dashboard/screens/reports",
    icon: faPieChart,
  },
];

export default navData;
