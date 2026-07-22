import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Dashboared } from './dashboared';

describe('Dashboared', () => {
  let component: Dashboared;
  let fixture: ComponentFixture<Dashboared>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Dashboared],
    }).compileComponents();

    fixture = TestBed.createComponent(Dashboared);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
