import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { ForgotPassword } from './features/auth/forgot-password/forgot-password';
import { ResetPassword } from './features/auth/reset-password/reset-password';

import { AdminDashboard } from './features/dashboard/admin-dashboard/admin-dashboard';
import { HrDashboard } from './features/dashboard/hr-dashboard/hr-dashboard';
import { ManagerDashboard } from './features/dashboard/manager-dashboard/manager-dashboard';
import { EmployeeDashboard } from './features/dashboard/employee-dashboard/employee-dashboard';

import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';
import { MainLayout } from './layouts/main-layout/main-layout';

import { Employees } from './features/admin/employees/employees';
import { Departments } from './features/admin/departments/departments';
import { LeaveTypes } from './features/admin/leave-types/leave-types';
import { Holidays } from './features/admin/holidays/holidays';
import { Goals } from './features/admin/goals/goals';
import { Reviews } from './features/admin/reviews/reviews';
import { Resignations } from './features/admin/resignations/resignations';
import { Payroll } from './features/admin/payroll/payroll';
import { EmployeeDetails } from './features/admin/employee-details/employee-details';
import { Leave } from './features/leave/leave';
import { Attendance } from './features/attendance/attendance';
import { SalaryStructures } from './features/salary-structures/salary-structures';
import { AdditionsDeductions } from './features/additions-deductions/additions-deductions';
import { PerformanceBonus } from './features/performance-bonus/performance-bonus';
import { EmployeeSalaries } from './features/employee-salaries/employee-salaries';

import { ManagerTeam } from './features/manager/team/manager-team';
import { ManagerAttendance } from './features/manager/attendance/manager-attendance';
import { ManagerLeave } from './features/manager/leave/manager-leave';
import { ManagerGoals } from './features/manager/goals/manager-goals';
import { ManagerReviews } from './features/manager/reviews/manager-reviews';

import { EmpAttendance } from './features/employee/attendance/emp-attendance';
import { EmpLeave } from './features/employee/leave/emp-leave';
import { EmpPayroll } from './features/employee/payroll/emp-payroll';
import { EmpGoals } from './features/employee/goals/emp-goals';
import { EmpReviews } from './features/employee/reviews/emp-reviews';
import { EmpResignation } from './features/employee/resignation/emp-resignation';
import { EmpProfile } from './features/employee/profile/emp-profile';
import { OrgChartComponent } from './features/org-chart/org-chart';

import { Overview } from './features/admin/employee-details/overview/overview';
import { Addresses } from './features/admin/employee-details/addresses/addresses';
import { Educations } from './features/admin/employee-details/educations/educations';
import { Experiences } from './features/admin/employee-details/experiences/experiences';
import { EmergencyContacts } from './features/admin/employee-details/emergency-contacts/emergency-contacts';
import { Documents } from './features/admin/employee-details/documents/documents';
import { Salary } from './features/admin/employee-details/salary/salary';
import { LeaveBalances } from './features/admin/employee-details/leave-balances/leave-balances';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    component: Login
  },

  {
    path: 'forgot-password',
    component: ForgotPassword
  },

  {
    path: 'reset-password',
    component: ResetPassword
  },

  {
  path: '',
  component: MainLayout,
  canActivate: [authGuard],
  children: [

    {
      path: 'admin/dashboard',
      component: AdminDashboard,
      canActivate: [roleGuard],
      data: {
        roles: ['Admin']
      }
    },

    {
  path: 'admin/employees',
  component: Employees,
  canActivate: [roleGuard],
  data: {
    roles: ['Admin']
  }
  },

  {
    path: 'admin/departments',
    component: Departments,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },

  {
    path: 'admin/leave-types',
    component: LeaveTypes,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },

  {
    path: 'admin/holidays',
    component: Holidays,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },

  {
    path: 'admin/goals',
    component: Goals,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },

  {
    path: 'admin/reviews',
    component: Reviews,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },

  {
    path: 'admin/resignations',
    component: Resignations,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },

  {
    path: 'admin/payroll',
    component: Payroll,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },
  {
    path: 'admin/employee-salaries',
    component: EmployeeSalaries,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },
  {
    path: 'admin/salary-structures',
    component: SalaryStructures,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },
  {
    path: 'admin/additions-deductions',
    component: AdditionsDeductions,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },
  {
    path: 'admin/performance-bonus',
    component: PerformanceBonus,
    canActivate: [roleGuard],
    data: {
      roles: ['Admin']
    }
  },
  {
  path: 'admin/employees/:id',
  component: EmployeeDetails,
  canActivate: [roleGuard],
  data: {
    roles: ['Admin']
  }
},
{
  path: 'admin/employee-details',
  component: EmployeeDetails,
  canActivate: [roleGuard],
  data: {
    roles: ['Admin']
  },
  children: [

    {
      path: '',
      redirectTo: 'overview',
      pathMatch: 'full'
    },

    {
      path: 'overview',
      component: Overview
    },

    {
      path: 'addresses',
      component: Addresses
    },

    {
      path: 'educations',
      component: Educations
    },

    {
      path: 'experiences',
      component: Experiences
    },

    {
      path: 'emergency-contacts',
      component: EmergencyContacts
    },

    {
      path: 'documents',
      component: Documents
    },
    {
      path: 'salary',
      component: Salary
    },
    {
      path: 'leave-balances',
      component: LeaveBalances
    }

  ]
},

    {
      path: 'hr/dashboard',
      component: HrDashboard,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/employees',
      component: Employees,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/employees/:id',
      component: EmployeeDetails,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/employee-details',
      component: EmployeeDetails,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      },
      children: [
        {
          path: '',
          redirectTo: 'overview',
          pathMatch: 'full'
        },
        {
          path: 'overview',
          component: Overview
        },
        {
          path: 'addresses',
          component: Addresses
        },
        {
          path: 'educations',
          component: Educations
        },
        {
          path: 'experiences',
          component: Experiences
        },
        {
          path: 'emergency-contacts',
          component: EmergencyContacts
        },
        {
          path: 'documents',
          component: Documents
        },
        {
          path: 'salary',
          component: Salary
        },
        {
          path: 'leave-balances',
          component: LeaveBalances
        }
      ]
    },
    {
      path: 'hr/leave',
      component: Leave,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/attendance',
      component: Attendance,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/payroll',
      component: Payroll,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/employee-salaries',
      component: EmployeeSalaries,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/salary-structures',
      component: SalaryStructures,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/additions-deductions',
      component: AdditionsDeductions,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/performance-bonus',
      component: PerformanceBonus,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/holidays',
      component: Holidays,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/leave-types',
      component: LeaveTypes,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/reviews',
      component: Reviews,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },
    {
      path: 'hr/resignations',
      component: Resignations,
      canActivate: [roleGuard],
      data: {
        roles: ['HR']
      }
    },

    {
      path: 'manager/dashboard',
      component: ManagerDashboard,
      canActivate: [roleGuard],
      data: {
        roles: ['Manager']
      }
    },
    {
      path: 'manager/team',
      component: ManagerTeam,
      canActivate: [roleGuard],
      data: {
        roles: ['Manager']
      }
    },
    {
      path: 'manager/team-attendance',
      component: ManagerAttendance,
      canActivate: [roleGuard],
      data: {
        roles: ['Manager']
      }
    },
    {
      path: 'manager/leave-approvals',
      component: ManagerLeave,
      canActivate: [roleGuard],
      data: {
        roles: ['Manager']
      }
    },
    {
      path: 'manager/goals',
      component: ManagerGoals,
      canActivate: [roleGuard],
      data: {
        roles: ['Manager']
      }
    },
    {
      path: 'manager/reviews',
      component: ManagerReviews,
      canActivate: [roleGuard],
      data: {
        roles: ['Manager']
      }
    },

    {
      path: 'employee/dashboard',
      component: EmployeeDashboard,
      canActivate: [roleGuard],
      data: { roles: ['Employee'] }
    },
    {
      path: 'employee/attendance',
      component: EmpAttendance,
      canActivate: [roleGuard],
      data: { roles: ['Employee'] }
    },
    {
      path: 'employee/leave',
      component: EmpLeave,
      canActivate: [roleGuard],
      data: { roles: ['Employee'] }
    },
    {
      path: 'employee/payroll',
      component: EmpPayroll,
      canActivate: [roleGuard],
      data: { roles: ['Employee'] }
    },
    {
      path: 'employee/goals',
      component: EmpGoals,
      canActivate: [roleGuard],
      data: { roles: ['Employee'] }
    },
    {
      path: 'employee/reviews',
      component: EmpReviews,
      canActivate: [roleGuard],
      data: { roles: ['Employee'] }
    },
    {
      path: 'employee/resignation',
      component: EmpResignation,
      canActivate: [roleGuard],
      data: { roles: ['Employee'] }
    },
    {
      path: 'employee/profile',
      component: EmpProfile,
      canActivate: [roleGuard],
      data: { roles: ['Employee'] }
    },
    {
      path: 'org-chart',
      component: OrgChartComponent
    }

  ]
},

  {
    path: '**',
    redirectTo: ''
  }
];