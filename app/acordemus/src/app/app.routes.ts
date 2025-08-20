
import { Routes } from '@angular/router';

export const routes: Routes = [
	{
		path: 'nova-proposta',
		loadComponent: () => import('./featured/components/new-proposal/new-proposal').then(m => m.NewProposal)
	},
    {
        path: '',
        loadComponent: () => import('./core/components/home/home').then(m => m.Home)
    }
];
