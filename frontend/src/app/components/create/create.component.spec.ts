import { ComponentFixture, TestBed } from '@angular/core/testing';

import { provideHttpClient } from '@angular/common/http';
import { CreateLedgerComponent } from './create.component';

describe('CreateLedgerComponent', () => {
    let component: CreateLedgerComponent;
    let fixture: ComponentFixture<CreateLedgerComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CreateLedgerComponent],
            providers: [provideHttpClient()],
        }).compileComponents();

        fixture = TestBed.createComponent(CreateLedgerComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
