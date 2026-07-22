import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HotelInformation } from './hotel-information';

describe('HotelInformation', () => {
  let component: HotelInformation;
  let fixture: ComponentFixture<HotelInformation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HotelInformation],
    }).compileComponents();

    fixture = TestBed.createComponent(HotelInformation);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
