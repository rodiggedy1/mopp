import { Routes } from '@angular/router';
import { ApplyJobStepperComponent } from './components/apply-job-stepper/apply-job-stepper.component';
import { CandidatesComponent } from './components/candidates/candidates.component';
import { HiredComponent } from './components/hired/hired.component';
import { ScreeningBoardComponent } from './components/screening-board/screening-board.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { AdminFormsComponent } from './components/admin-forms/admin-forms.component';
import { JobSubmittedComponent } from './components/apply-job-stepper/job-submitted/job-submitted.component';
import { CandidateDetailsComponent } from './components/candidate-details/candidate-details.component';
import { SubscriptionComponent } from './components/subscription/subscription.component';
import { PaymentSuccessComponent } from './components/payment-success/payment-success.component';
import { PaymentCancelComponent } from './components/payment-cancel/payment-cancel.component';
import { AuthGuard } from './guards/auth-guard';
import { SchedulesComponent } from './components/schedules/schedules.component';
import { JobSubmittedQualifiedComponent } from "./components/apply-job-stepper/job-submitted-qualified/job-submitted-qualified.component";

export const routes: Routes = [
    {
        path: '',
        redirectTo: '/auth',
        pathMatch: 'full'
    },
    {
        path: 'apply/:id',
        component: ApplyJobStepperComponent
    },
    {
        path: 'job-submitted',
        component: JobSubmittedComponent
    },
    {
        path: 'job-submitted-qualified',
        component: JobSubmittedQualifiedComponent
    },
    {
        path: 'auth',
        loadChildren: () => import('./auth/auth.routes').then(m => m.AUTH_ROUTES),
    },
    {
      path: 'candidates',
      component: CandidatesComponent,
      canActivate: [AuthGuard],
    },
    {
      path: 'candidates/:id',
      component: CandidateDetailsComponent,
      canActivate: [AuthGuard],
    },
    {
      path: 'hired',
      component: HiredComponent,
      canActivate: [AuthGuard],
    },
    {
      path: 'screening',
      component: ScreeningBoardComponent,
      canActivate: [AuthGuard],
    },
    {
      path: 'dashboard',
      component: AdminDashboardComponent,
      canActivate: [AuthGuard],
    },
    {
      path: 'forms',
      component: AdminFormsComponent,
      canActivate: [AuthGuard],
    },
    {
      path: 'subscription',
      component: SubscriptionComponent
    },
    {
      path: 'payment-success',
      component: PaymentSuccessComponent
    },
    {
      path: 'payment-cancelled',
      component: PaymentCancelComponent
    },
    {
      path: 'schedules',
      component: SchedulesComponent
    },
    {
        path: '**',
        redirectTo: '/auth'
    },
];
